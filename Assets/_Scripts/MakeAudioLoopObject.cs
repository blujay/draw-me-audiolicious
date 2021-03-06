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

[RequireComponent(typeof(AudioSource))]

public class MakeAudioLoopObject : MonoBehaviourPun
{

    public int loopDuration;
    public Color recordingColor;
    public Color colorPostLoop;
    private string _SelectedDevice;
    private string[] devices;
    static float[] samplesData;

    [NonSerialized] public bool recording;
    [NonSerialized] public bool generated;
    private string filename;
    private string filepath;

    private float loopLenght = 0;

    private void OnEnable()
    {
        generated = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioSource audioS = GetComponent<AudioSource>();
        recording = false;
        generated = false;
        devices = Microphone.devices;
        if (PhotonNetwork.IsConnected)
        {
            Recorder recorder = GetComponent<Recorder>();
            recorder.Init(FindObjectOfType<VoiceConnection>());
            Speaker speaker = GetComponent<Speaker>();
            recorder.SourceType = Recorder.InputSourceType.AudioClip;
            audioS.clip = recorder.AudioClip;
            audioS.Play();
        }
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
            if (PhotonNetwork.IsConnected)
            {
                Recorder recorder = GetComponent<Recorder>();
                Speaker speaker = GetComponent<Speaker>();
                recorder.AudioClip = DownloadHandlerAudioClip.GetContent(www);
                recorder.AudioClip.name = filename;
                recorder.RestartRecording();
                if (GetComponent<PhotonVoiceView>().IsSpeakerLinked)
                {
                    speaker.RestartPlayback();
                }
                audioS.clip = recorder.AudioClip;
                audioS.clip.name = recorder.AudioClip.name;
                audioS.Play();

            }

            else
            {
                audioS.clip = DownloadHandlerAudioClip.GetContent(www);
                audioS.clip.name = filename;
                audioS.Play();

            }
            audioS.loop = true;
            generated = true;

        }
    }

    private void Update()
    {
        if (recording)
        {
            loopLenght += Time.deltaTime;
        }
        else
        {
            loopLenght = 0;
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
                recorder.SourceType = Recorder.InputSourceType.AudioClip;
                recorder.AudioClip = Microphone.Start(_SelectedDevice, false, 30, 48000); // third argument restrict the duration of the audio to duration specified
                while (!(Microphone.GetPosition(null) > 0)) { }
                samplesData = new float[recorder.AudioClip.samples * recorder.AudioClip.channels];
                recorder.AudioClip.GetData(samplesData, 0);
            }
            audioS.clip = Microphone.Start(_SelectedDevice, true, 30, 48000);  // third argument restrict the duration of the audio to duration specified
            while (!(Microphone.GetPosition(null) > 0)) { }
            samplesData = new float[audioS.clip.samples * audioS.clip.channels];
            audioS.clip.GetData(samplesData, 0);
        }

    }

    [Obsolete]
    public void StopRecording()
    {
        if (recording)
        {
            GetComponentInChildren<Renderer>().material.color = colorPostLoop;
            Debug.Log(filename);
            AudioSource audioS = this.gameObject.GetComponent<AudioSource>();


            // Delete the file if it exists.
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            try
            {

                Microphone.End(_SelectedDevice);

                recording = false;

                if (!recording && !generated)
                {
                    filename = (gameObject.name + "-" + GetComponent<PhotonView>().ViewID);
                    filepath = Path.Combine(Application.persistentDataPath, filename + ".wav");

                    AudioClip trimmedClip = ShortenAudioclip(audioS.clip, _SelectedDevice);

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

        AudioClip ShortenAudioclip(AudioClip recordedClip, string deviceName)
        {

            var position = Microphone.GetPosition(deviceName);
            var soundData = new float[recordedClip.samples * recordedClip.channels];
            recordedClip.GetData(soundData, 0);

            //Create shortened array for the data that was used for recording
            var newData = new float[position * recordedClip.channels];

            //Copy the used samples to a new array
            for (int i = 0; i < newData.Length; i++)
            {
                newData[i] = soundData[i];
            }

            //One does not simply shorten an AudioClip,
            //    so we make a new one with the appropriate length
            var newClip = AudioClip.Create(recordedClip.name,
                                            position,
                                            recordedClip.channels,
                                            recordedClip.frequency,
                                            false,
                                            false);

            newClip.SetData(newData, 0);        //Give it the data from the old clip

            //Replace the old clip
            AudioClip.Destroy(recordedClip);
            recordedClip = newClip;
            return recordedClip;
        }

    }

}
