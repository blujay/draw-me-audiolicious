using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Networking;

public class MakeAudioLoopObject : MonoBehaviour
    {

        public float loudness;
        public float sensitivity;
        private static AudioSource audioS;
        private AudioClip GenClip;
        public GameObject prefab;
        public float minScale;
        private string _SelectedDevice;
        private string[] devices;
        static float[] samplesData;

        public bool recording;
        public bool generated;
        private string fileName;
        private string filePath;
        private AudioSource audioSGen;


    public float x, y, z;

    // Start is called before the first frame update
    void Start()
        {
            recording = false;
            generated = false;
            sensitivity = 100;
            minScale = 0.5f;
            _SelectedDevice = Microphone.devices[0].ToString();
            audioS = GetComponent<AudioSource>();
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

    public void JigglePrefab()
        {
            prefab.transform.localScale += new Vector3(loudness, loudness, loudness);
        }




    /*public void recordClip()
    {
        audioS.clip = Microphone.Start(_SelectedDevice, true, 1, 22050);
        audioS.loop = true;

        while (!(Microphone.GetPosition(null) > 0)) { }
        audioS.clip = audioS.clip;
        audioS.Play();

    }
    */

    IEnumerator GenerateAudiObject(string filePath, AudioClip GenClip)
    {

        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            GenClip = DownloadHandlerAudioClip.GetContent(www);
            audioS.clip = GenClip;
            GameObject.Instantiate(prefab);
            audioSGen = prefab.AddComponent<AudioSource>();
            audioSGen.clip = GenClip;
            audioSGen.Play();
            audioSGen.loop = true;
        }
    }

    public void StartRecording()
        {
            prefab.GetComponent<Renderer>().material.color = Color.blue;
            recording = true;
            audioS.clip = Microphone.Start(Microphone.devices[0], true, 4, 22050);  // third argument restrict the duration of the audio to 10 seconds 
            while (!(Microphone.GetPosition(null) > 0)) {}
            recording = false;
            prefab.GetComponent<Renderer>().material.color = Color.white;
            samplesData = new float[audioS.clip.samples * audioS.clip.channels];
            audioS.clip.GetData(samplesData, 0);
        }

        public void StopRecording()
        {
        if (!recording) { 
            
            fileName = ("AudioClip-" + DateTime.Now.ToString("yyyymmdd--HH-mm-ss"));
            filePath = Path.Combine(Application.persistentDataPath, fileName + ".wav");
            Debug.Log(fileName);

            // Delete the file if it exists.
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            try
            {
                SavWav.Save(fileName, audioS.clip);
                Debug.Log("File Saved Successfully at: " + filePath);
            }
            catch (DirectoryNotFoundException)
            {
                Debug.LogError("Please, Create a StreamingAssets Directory in the Assets Folder");
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);    //check for other Exceptions 
            }
            Microphone.End(_SelectedDevice);
            if (!generated)
            {
                StartCoroutine(GenerateAudiObject(filePath, audioS.clip));
                generated = true;
            }
        }

    }

}

