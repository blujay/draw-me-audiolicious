using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;
using System;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Pun.UtilityScripts;
using UnityEngine.Networking;
using Photon.Voice.PUN;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

//This script can be used with or without Pun Voice. Add it to an object and add a method of calling the method. 
//MakeyLoopButtons.cs will use the A button on controller to generate the audio loop
//MakeyLoopMouth.cs will use a mouth collider attached to the player head prefab to generate the audio when this object is triggered and saves file/plays it when exiting trigger.
//The script starts recording with the microphone, saves the recording into a wav file and stores it in the persistentDataPath as a file with same name as this object. 
//It then directly sets the clip as the input for the audio source.

//Todo - find the right way to get the pun recorder to transmit all audiosources in scene when input is an audioclip from the player who generated it... do these become 'players'?

/* Create motherloop
     *  First recording in a scene is the 'mother loop' this recording is used to syncronise any future loop with
     *  Mother loop playback starts immediately after recording stopped/saved
     *  Child loop = any subsequent loop
     * 
     *     Mother loop
     *     |-----------------------|-----------------------|-----------------------|-----------------------|
     *     
     *     Child loops (longer than & shorter than)
     *     |-------------------------------|               |-------------------------------|
     *          |---|                   |---|                   |---|                   |---|
     *        |---------------|       |---------------|       |---------------|       |---------------|
     * 
 */


[RequireComponent(typeof(AudioSource))]

public class MakeAudioLoopWithMother : MonoBehaviourPun
{

    public int loopDuration;
    public Color recordingColor;
    public Color colorPostLoop;
    private string _SelectedDevice;
    private string[] devices;
    private MotherLoopManager motherLooper;
    static float[] samplesData;

    [NonSerialized] public bool recording;
    [NonSerialized] public bool generated;
    private string filename;
    private string filepath;

    private void OnEnable()
    {
        generated = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioSource audioS = GetComponent<AudioSource>();
        motherLooper = FindObjectOfType<MotherLoopManager>();
        recording = false;
        generated = false;
        devices = Microphone.devices;


        if (PhotonNetwork.IsConnected)
        {
            Recorder recorder = GetComponent<Recorder>();
            //recorder.Init(FindObjectOfType<VoiceConnection>());
            recorder.SourceType = Recorder.InputSourceType.AudioClip;
            if (recorder.IsRecording)
            {
                StopRecording();
            }
            
            audioS.clip = recorder.AudioClip;
            Speaker speaker = GetComponent<Speaker>();
            if (speaker.IsPlaying)
            {
                speaker.RestartPlayback();
            }
        }
        audioS.Play();
        //set the recorder component to play from the new audioclip

    }


    IEnumerator GenerateAudioObject(string filepath, string filename, AudioClip GenClip)
    {
        AudioSource audioS = this.gameObject.GetComponent<AudioSource>();

        if (PhotonNetwork.IsConnected)
        {
            Recorder recorder = GetComponent<Recorder>();
            Speaker speaker = GetComponent<Speaker>();
        }


        if (Application.platform == RuntimePlatform.Android)
        {

            filepath = Path.Combine("file://" + Application.persistentDataPath, filename + ".wav");
            Debug.Log(filepath);
        }
        else
        {
            filepath = Path.Combine(Application.persistentDataPath, filename + ".wav");

        }

        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filepath, AudioType.WAV);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error); //file not found

        }
        else //file is found
        {
            StartPlayback(www);
        }
    }

    public void StartRecording()
    {
        if (!generated)
        {
            AudioSource audioS = this.gameObject.GetComponent<AudioSource>();
            GetComponentInChildren<Renderer>().material.color = recordingColor;
            recording = true;

            if (PhotonNetwork.IsConnected)
            {
                Recorder recorder = this.GetComponent<Recorder>();
                GetComponent<PhotonVoiceView>().UsePrimaryRecorder = true;
                recorder.StartRecording();
                //set the recorder component to record from the microphone
                recorder.SourceType = Recorder.InputSourceType.AudioClip;
                _SelectedDevice = recorder.UnityMicrophoneDevice = devices[0];
                recorder.AudioClip = Microphone.Start(_SelectedDevice, false, 30, 44100); // third argument restrict the duration of the audio to duration specified
                while (!(Microphone.GetPosition(null) > 0)) { }
                samplesData = new float[recorder.AudioClip.samples * recorder.AudioClip.channels];
                recorder.AudioClip.GetData(samplesData, 0);
            } else
            {
                _SelectedDevice = devices[0];
                audioS.clip = Microphone.Start(_SelectedDevice, false, 30, 44100);  // third argument restrict the duration of the audio to duration specified
                while (!(Microphone.GetPosition(null) > 0)) { }
                samplesData = new float[audioS.clip.samples * audioS.clip.channels];
                audioS.clip.GetData(samplesData, 0);
            }
            
        }

    }

    public void StopRecording()
    {
        if (recording)
        {
            GetComponentInChildren<Renderer>().material.color = colorPostLoop;
            Debug.Log(filename);
            AudioSource audioS = this.gameObject.GetComponent<AudioSource>();
            Recorder recorder = this.GetComponent<Recorder>();


            // Delete the file if it exists.
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            try
            {
                if (PhotonNetwork.IsConnected)
                {
                    
                    if (recorder.IsRecording)
                    {
                        recording = false;
                        recorder.StopRecording();
                        GetComponent<PhotonVoiceView>().UsePrimaryRecorder = false;
                    }
                    
                } else
                {
                    recording = false;
                    Microphone.End(_SelectedDevice);
                }
                    

                

                if (!recording && !generated)
                {
                    filename = (gameObject.name + "-" + GetComponent<PhotonView>().ViewID);
                    filepath = Path.Combine(Application.persistentDataPath, filename + ".wav");
                    AudioClip trimmedClip = SavWav.TrimSilence(audioS.clip, 0f);
                    SavWav.Save(filename, trimmedClip);
                    Debug.Log("File Saved Successfully at: " + filepath);
                }

            }
            catch (DirectoryNotFoundException)
            {
                Debug.LogError("this is not the folder you need. It will appear in the application.persistentDatapath folder. ");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);    //check for other Exceptions 
            }

            if (!generated && !recording)
            {
                StartCoroutine(GenerateAudioObject(filepath, filename, audioS.clip)); ;
                generated = true;
            }
        }

    }

    public void talkToMotherLooper(AudioSource audioS, AudioClip clip)
    {
        audioS = GetComponent<AudioSource>();
        clip = clip;
    }

    public void StartPlayback(UnityWebRequest www)
    {
        AudioSource audioS = GetComponent<AudioSource>();

        if (PhotonNetwork.IsConnected)
        {
            Recorder recorder = GetComponent<Recorder>();
            Speaker speaker = GetComponent<Speaker>();
            recorder.AudioClip = DownloadHandlerAudioClip.GetContent(www);
            recorder.AudioClip.name = filename;
            if(recorder.IsRecording == false)
            {
                recorder.RestartRecording();
                recorder.IsRecording = true;
            }
            
            if (GetComponent<PhotonVoiceView>().IsSpeakerLinked)
            {
                speaker.RestartPlayback();
            }
            audioS.clip = recorder.AudioClip;
            audioS.clip.name = recorder.AudioClip.name;
        }

        else
        {
            audioS.clip = DownloadHandlerAudioClip.GetContent(www);
            audioS.clip.name = filename;
        }

        audioS.loop = true;
        generated = true;
        audioS.Stop();
        audioS.Play();
    }

}
