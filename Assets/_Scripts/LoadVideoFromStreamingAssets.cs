using UnityEngine;
using UnityEngine.Video;


// ExecuteInEditMode can run scripts without running programs
// Attribute link: https://blog.csdn.net/qq_39097425/article/details/109194397
// [ExecuteInEditMode]  
[RequireComponent(typeof(VideoPlayer))]

public class LoadVideoFromStreamingAssets : MonoBehaviour
{
    public bool LoadFromStreamingAssets = true;
    public string URL;

    private void Start()
    {
        VideoPlayer vp = gameObject.GetComponent<VideoPlayer>();
        if (vp != null)
        {
            vp.source = VideoSource.Url;
            vp.playOnAwake = true;
            vp.url = Application.streamingAssetsPath + "/" + URL.Replace(@"\", "/");
            vp.Play();

        }
    }

}
