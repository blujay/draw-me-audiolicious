﻿using System.Collections;
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
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

[RequireComponent(typeof(AudioSource))]

public class MakeAudioLoopObject : MonoBehaviourPun
    {

        //public float loudness;
        //public float sensitivity;
        public int loopDuration;
        public Color recordingColor;
        public Color colorPostLoop;
        //public float minScale;
        private string _SelectedDevice;
        private string[] devices;
        static float[] samplesData;

        [NonSerialized] public bool recording;
        [NonSerialized] public bool generated;
        private string filename;
        private string filepath;

    private void OnEnable()
    {
        generated = false;
    }

    private Recorder punRecorder;
    private Speaker punSpeaker;
    // Start is called before the first frame update
    void Start()
    {
        
        recording = false;
        generated = false;
        devices = Microphone.devices;
        if (devices.Length > 0)
        {
            punRecorder = GetComponent<Recorder>();
            punSpeaker = GetComponent<Speaker>();
            punRecorder.Init(FindObjectOfType<VoiceConnection>());
            _SelectedDevice = punRecorder.UnityMicrophoneDevice = devices[0].ToString();
            punRecorder.SourceType = Recorder.InputSourceType.Microphone;
            

        }
       
        
    }

    
IEnumerator GenerateAudioObject(string filepath, string filename, AudioClip GenClip, Recorder recorder, Speaker speaker)
    {
        AudioSource audioS = this.gameObject.GetComponent<AudioSource>();
        recorder = GetComponent<Recorder>();
        speaker = GetComponent<Speaker>();
        

        if (Application.platform == RuntimePlatform.Android)
        {
            
            filepath = Path.Combine("file://" + Application.persistentDataPath, filename + ".wav");
            Debug.Log (filepath);
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

            //load the newly generated and saved clip using the www request 
            audioS = gameObject.GetComponent<AudioSource>();
            //set the recorder component to play from the new audioclip
            punRecorder.SourceType = Recorder.InputSourceType.AudioClip;
            if (punSpeaker.IsPlaying)
            {
                Debug.Log("speaker is playing" + punSpeaker.name);
            }


            audioS.clip = DownloadHandlerAudioClip.GetContent(www);
            audioS.clip.name = filename;
            audioS.Play();
            audioS.loop = true;
            generated = true;
            recorder.AudioClip = audioS.clip;
            
        }
    }

    public void StartRecording()
        {
            if (!generated)
            {
                AudioSource audioS = this.gameObject.GetComponent<AudioSource>();
                GetComponentInChildren<Renderer>().material.color = recordingColor;
                recording = true;
                punRecorder.StartRecording();
                audioS.clip = Microphone.Start(_SelectedDevice, true, loopDuration, 48000);  // third argument restrict the duration of the audio to duration specified
                while (!(Microphone.GetPosition(null) > 0)) { }
                samplesData = new float[audioS.clip.samples * audioS.clip.channels];
                audioS.clip.GetData(samplesData, 0);
            }
            
        }

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
                punRecorder.StopRecording();
                recording = false;
                if (!recording && !generated)
                {
                    filename = (gameObject.name + "-" + GetComponent<PhotonView>().ViewID);
                    filepath = Path.Combine(Application.persistentDataPath, filename + ".wav");
                    SavWav.Save(filename, audioS.clip);
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
                StartCoroutine(GenerateAudioObject(filepath, filename, audioS.clip, punRecorder, punSpeaker)); ;
                generated = true;
            }
        }

    }

}

