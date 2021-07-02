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
        public GameObject prefab;
        public int loopDuration;
        //public float minScale;
        private string _SelectedDevice;
        private string[] devices;
        static float[] samplesData;

        public bool recording;
        public bool generated;
        private string filename;
        private string filepath;
        private AudioSource audioSGen;


    public float x, y, z;

    // Start is called before the first frame update
    void Start()
        {
            recording = false;
            generated = false;
            sensitivity = 100;
            loopDuration = 4;
            //minScale = 0.5f;
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

IEnumerator GenerateAudiObject(string filepath, string filename, AudioClip GenClip)
    {
        filepath = Path.Combine(Application.persistentDataPath, filename + ".wav");
        

        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filepath, AudioType.WAV);
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
            prefab.transform.position = new Vector3(transform.position.x + 1f, transform.position.y + 1f, transform.position.z + 1f);
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
            audioS.clip = Microphone.Start(Microphone.devices[0], true, loopDuration, 22050);  // third argument restrict the duration of the audio to 10 seconds 
            while (!(Microphone.GetPosition(null) > 0)) {}
            samplesData = new float[audioS.clip.samples * audioS.clip.channels];
            audioS.clip.GetData(samplesData, 0);
        }

    public void StopRecording()
    {
        prefab.GetComponent<Renderer>().material.color = Color.white;
        Debug.Log(filename);

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
                filename = ("AudioClip-" + DateTime.Now.ToString("yyyymmdd--HH-mm-ss"));
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
            prefab.GetComponent<Collider>().isTrigger = false;
        }

    }

}

