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
using Networking.Pun2;
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

    public Color recordingColor;
    public Color colorPostLoop;
    private string _SelectedDevice;
    private string[] devices;
    private AudioSource audioS;
    private MotherLoopManager motherLooper;
    private float motherLoopPos;
    static float[] samplesData;
    private float loopLength;

    [NonSerialized] public bool recording;
    [NonSerialized] public bool generated;
    private string filename;
    private string filepath;

    private void OnEnable()
    {
        generated = false;
        loopLength = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        motherLooper = FindObjectOfType<MotherLoopManager>();
        recording = false;
        generated = false;
        devices = Microphone.devices;

        //set the recorder component to play from the new audioclip

    }






    void Update()
    {
        if (gameObject.GetComponent<PunOVRGrabbable>().isGrabbed)
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                generated = false;
                StartRecording();
            }

            if (OVRInput.GetUp(OVRInput.Button.One))
            {
                StopRecording();
            }
        }

        //if (gameObject.GetComponent<PunOVRGrabbable>().isGrabbed == false)
        //{
        //    Debug.Log("Dropped");
        //    //GetComponent<MakeAudioLoopWithMother>().StopRecording();
        //}

    }







    //private void Update()
    //{
    //    if (recording)
    //    {
    //        loopLength += Time.deltaTime;
    //    }
    //    else
    //    {
    //        loopLength = 0;
    //    }
    //}

    
    IEnumerator GenerateAudioObject(string filename)
    {

        AudioSource audioS = this.gameObject.GetComponent<AudioSource>();

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
            audioS.clip = audioS.clip = DownloadHandlerAudioClip.GetContent(www);
            audioS.clip.name = filename;
            motherLooper.AddClip(audioS, audioS.clip, motherLoopPos, true);
        }
    }

    public void StartRecording()
    {
        if (!generated)
        {
            AudioSource audioS = gameObject.GetComponent<AudioSource>();
            GetComponentInChildren<Renderer>().material.color = recordingColor;

            _SelectedDevice = devices[0];
            audioS.clip = Microphone.Start(_SelectedDevice, false, 30, 48000);  // third argument restrict the duration of the audio to duration specified
            recording = true;
            //what position is the motherloop at right now?
            if (motherLooper.MotherExists && motherLooper.motherAudioSource.isPlaying)
            {
                motherLoopPos = (float) motherLooper.motherAudioSource.timeSamples / motherLooper.motherClip.frequency; //in this example right now it would be at 3s
            }
            while (!(Microphone.GetPosition(null) > 0)) { }
            samplesData = new float[audioS.clip.samples * audioS.clip.channels];
            audioS.clip.GetData(samplesData, 0);
        }
    }

    public void StopRecording()
    {
        if (recording)
        {
            recording = false;
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

                if (!recording && !generated)
                {
                    filename = (gameObject.name + "-" + GetComponent<PhotonView>().ViewID);
                    filepath = Path.Combine(Application.persistentDataPath, filename + ".wav");
                    AudioClip trimmedClip = SavWav.TrimSilence(audioS.clip, 0f);
                    SavWav.Save(filename, trimmedClip);
                    audioS.clip = trimmedClip;
                    audioS.clip.name = filename;
                    Debug.Log("loop duration events should be - " + loopLength); // seems to be the difference between recorded and trimmed. maybe delay in running co-routines?
                    Debug.Log("loop duration trimmed clip is - " + audioS.clip.length);
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
                StartCoroutine(GenerateAudioObject(filename));
                generated = true;
            }
        }
    }
}
