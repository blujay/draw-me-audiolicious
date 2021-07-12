using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceToClip : MonoBehaviour
{

    // Start recording with built-in Microphone and play the recorded audio right away

    private AudioClip myClip;
    public AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
        myClip = Microphone.Start("Built-in Microphone", true, 10, 44100);
        source.loop = true;
        source.clip = myClip;
       
    }

// Update is called once per frame
void Update()
    {
        while (!(Microphone.GetPosition(null) > 0)) { }

        source.Play();
    }
}
