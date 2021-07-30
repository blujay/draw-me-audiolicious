using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/*Motherloop manager keeps track of all the loops that are recorded in the scene 
 * Requires MakeAudioLoop.cs to be put on each object that will be recording a loop
     *  First recording in a scene is the 'mother loop' this recording is used to syncronise any future loop with
     *  Mother loop playback starts immediately after recording stopped/saved
     *  Child loop = any subsequent loop is a child loop and the delay from start of motherloop is added to it's length
     *  Child loop will start to play at the next available mother loop given how long it is. If it's longer than mother, it will skip some.
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

public class MotherLoopManager : MonoBehaviour
{
    [ReadOnly] public AudioSource motherAudioSource;
    [ReadOnly] public AudioClip motherClip;    
    
    public struct ChildLoop
    {
        public AudioClip Clip;
        public float Delay;
        public int PlayEvery;
        public AudioSource Source;
    }

    public List<ChildLoop> ChildLoops;
    private int LoopCounter;

    public void AddClip(AudioSource source, AudioClip clip, float delay)
    {
        if (motherAudioSource == null) // We are adding the mother
        {
            motherAudioSource = source;
            motherClip = clip;
            InvokeRepeating(nameof(PlayChildLoops), 0, motherClip.length);
            motherAudioSource.clip = motherClip;
            motherAudioSource.loop = true;
            motherAudioSource.Play();
        }
        else  // We are adding a new child
        {
            int playEvery;
            float clipTotalLength = delay + clip.length;
            playEvery = Mathf.FloorToInt(motherClip.length / clipTotalLength) + 1;
            
            var newChild = new ChildLoop
            {
                Clip = clip,
                Delay = delay,
                PlayEvery = playEvery,
                Source = source,
            };
            ChildLoops.Add(newChild);
        }
    }
    
    private void PlayChildLoops()
    {
        foreach (var loop in ChildLoops)
        {
            if (LoopCounter % loop.PlayEvery == 0)
            {
                loop.Source.clip = loop.Clip;
                loop.Source.PlayDelayed(loop.Delay);
            }
        }
        LoopCounter++;
    }
}
