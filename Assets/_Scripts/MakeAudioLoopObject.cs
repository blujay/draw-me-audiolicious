using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;
using System;
using Photon.Pun;
using UnityEngine.Networking;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class MakeAudioLoopObject : MonoBehaviour
    {

        //public float loudness;
        //public float sensitivity;
        public int loopDuration;
        //public float minScale;
        private string _SelectedDevice;
        private string[] devices;
        static float[] samplesData;

        public bool recording;
        public bool generated;
        private string filename;
        private string filepath;
        //private string packageName;
        //public Text pathtext;
        //public float x, y, z;

    // Start is called before the first frame update
    void Start()
    {
        recording = false;
        generated = false;
        
        //sensitivity = 100;
        loopDuration = 4;
        //minScale = 0.5f;
        _SelectedDevice = Microphone.devices[0].ToString();
        //packageName = "com." + Application.companyName + "." + Application.productName;
        //Debug.Log(packageName);
    }

    // Update is called once per frame
    void Update()
        {
            //loudness = GetAverageVolume() * sensitivity;
            //loudness += loudness;
        }

    /*float GetAverageVolume()
    {
        float[] data = new float[256];
        float a = 0;
        audioSGen.GetOutputData(data, 0);
        foreach (float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a / 256;
    }*/

    //public void JigglePrefab()
        //{
            //prefab.transform.localScale += new Vector3(loudness, loudness, loudness);
        //}


IEnumerator GenerateAudiObject(string filepath, string filename, AudioClip GenClip)
    {
        AudioSource audioS = this.gameObject.GetComponent<AudioSource>();

        if (Application.platform == RuntimePlatform.Android)
        {
            
            filepath = Path.Combine("file://" + Application.persistentDataPath, filename + ".wav");
            Debug.Log (filepath);
            //pathtext.text = (File.Exists(filepath) ? "Android - File exists at" + filepath : "File does not exist at" + filepath);
                    }
        else
        {
            filepath = Path.Combine(Application.persistentDataPath, filename + ".wav");
            //pathtext.text = (File.Exists(filepath) ? "PC - File exists at" + filepath : "File does not exist at" + filepath);
        }
       


        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filepath, AudioType.WAV);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
            //pathtext.text = www.error;
        }
        else //we've got it! no errors. The file is found
        {

            //load the newly generated and saved clip using the www request 
            audioS = gameObject.GetComponent<AudioSource>();
            audioS.clip = DownloadHandlerAudioClip.GetContent(www);
            audioS.clip.name = filename;
            audioS.Play();
            audioS.loop = true;
            generated = true;
        }
    }

    public void StartRecording()
        {
            if (!generated)
            {
                AudioSource audioS = this.gameObject.GetComponent<AudioSource>();
                GetComponentInChildren<Renderer>().material.color = Color.blue;
                recording = true;
                audioS.clip = Microphone.Start(Microphone.devices[0], true, loopDuration, 22050);  // third argument restrict the duration of the audio to 10 seconds 
                while (!(Microphone.GetPosition(null) > 0)) { }
                samplesData = new float[audioS.clip.samples * audioS.clip.channels];
                audioS.clip.GetData(samplesData, 0);
            }
            
        }

    public void StopRecording()
    {
        if (recording)
        {
            GetComponentInChildren<Renderer>().material.color = Color.white;
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
                    filename = ("clip" + DateTime.Now.ToString("yyyymmdd--HH-mm-ss"));
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
                StartCoroutine(GenerateAudiObject(filepath, filename, audioS.clip));
                generated = true;
            }
        }

    }

}

