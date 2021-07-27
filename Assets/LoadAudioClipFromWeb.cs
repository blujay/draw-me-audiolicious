using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class LoadAudioClipFromWeb: MonoBehaviour
{

    public AudioSource audioS;
    public GameObject loadingBar;
    public UnityWebRequest loadingwww;
    private string audioClipUrl;
    public InputField inputField;

    public void LoadAudioFromWeb()
    {
        StartCoroutine(GetAudioClip());
    }


    IEnumerator GetAudioClip()
    {
        audioClipUrl = inputField.text;
        Debug.Log(audioClipUrl);
        using (loadingwww = UnityWebRequestMultimedia.GetAudioClip(audioClipUrl, AudioType.WAV))
        {
            yield return loadingwww.SendWebRequest();

            if (loadingwww.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(loadingwww.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(loadingwww);
                if (loadingwww.downloadProgress <1f)
                {
                    loadingBar.transform.localScale = new Vector3(loadingwww.downloadProgress, 1, 1);
                }
                audioS.clip = myClip;
                myClip.name = audioClipUrl;
                audioS.loop = true;
                audioS.Play();
            }
        }
    }
}