using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

//works on an object with an AudioSource component and a file called testwav.wav saved in the persistent file path. Which is a pc is: C:/Users/userAccount/AppData/LocalLow/companyName/appName

public class LoadAudioClip : MonoBehaviour
{
    string filepath;
    string filename;
    private AudioSource audioS;
    void Start()
    {
        StartCoroutine(GetAudioClip());
        audioS = GetComponent<AudioSource>();
    }

    IEnumerator GetAudioClip()
    {
        filename = "testwav.wav"; //replace or ensure you have a test wav file called testwav.wav
        filepath = Path.Combine(Application.persistentDataPath, filename);
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filepath, AudioType.WAV);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
            audioS.clip = myClip;
            audioS.loop = true;
            audioS.Play();
        }
    }
}