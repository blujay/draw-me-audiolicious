using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/*Motherloop manager keeps track of all the loops that are recorded in the scene 
 * Requires MakeAudioLoopWithMother.cs to be put on each object that will be recording a loop
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
     * 
     * Thanks to Andy Baker @andybak for helping make the motherlooper on a sunday afternoon, 
     * to Oliver D'Adda for suggesting the Motherlooper approach, 
     * to @BuntyLooping for wanting it to work like that
 */

public class MotherLoopManager : MonoBehaviour
{
    [ReadOnly] public AudioSource motherAudioSource;
    [ReadOnly] public AudioClip motherClip;
    [ReadOnly] public float motherClipLength;
    [ReadOnly] public float motherClipPosSamples;
    public AudioClip DebugClip;
    public AudioSource DebugSource;
    //public AudioSource motherAudioSource;
    //public AudioClip motherClip;
    //public AudioSource testChildSource;
    //public AudioClip testChildClip;

    [Serializable]
    public struct ChildLoop
    {
        public AudioClip Clip;
        public int PlayEvery;
        public bool PlayNext;
        public AudioSource Source;
        public float Delay;
        public float MotherLoopPos;
        public int LoopCounter;
    }

    public List<ChildLoop> ChildLoops;
    public float DelayMod;
    public bool MotherExists;
    

    private void Start()
    {
        ChildLoops = new List<ChildLoop>();
        MotherExists = false;
    }

    private void Update()
    {
        if(MotherExists == true)
        {
            motherClipPosSamples = (float) motherAudioSource.timeSamples / motherClip.frequency;
        } 
    }

    public void AddClip(AudioSource source, AudioClip clip, float delay, bool playNext)
    {
        if (motherAudioSource == null) // We are adding the mother
        {
            MotherExists = true;
            motherAudioSource = source;
            motherClip = clip;
            motherAudioSource.loop = false;
            motherClipLength = motherClip.length;
            motherAudioSource.clip = motherClip;
            motherAudioSource.clip.name = clip.name;
        }
        else  

        {
            float childEndTime = delay + clip.length;

            //calculate the maths for playEvery
            //examples

            //|---------|---------|---------|---------|// lets say it's 10s
            //|--x..----|..x..----|--x..----|--x..----| x is where our child loop starts recording  - lets say it's at 3s and our child loop is 3s long so there is 4s left
            //|--x..........------|--x..........------| child clip now starts at 3s and lasts 11s so overlaps motherloop by 2s
            //|--x....................------|--x....... child clip is longer than 2 mother clips. starts at 3s and lasts 20s so PlayEvery=3

            //case 1 - if child loop fits into what's left of this motherloop as in our example...PlayEvery=1; (10-20 = -10) but duration of this is 30secs which is 
            // clip.length = 3
            // delay = 3;
            // childEndTime = 6
            // motherClip.length = 10
            // playEvery = 1;

            //case 2 - child loop is longer than motherloop. it has to wait until next loop. PlayEvery=2
            //clip.length = 11
            // delay = 3;
            // childEndTime = 14
            // motherClip.length = 10
            // playEvery = 2;

            //case 3 - child loop is longer than 2 x motherloop. it has to wait until next loop. PlayEvery=2
            //clip.length = 20
            // delay = 3;
            // childEndTime = 23
            // motherClip.length = 10
            // playEvery = 3;

            int playEvery = Mathf.FloorToInt(childEndTime / motherClip.length) + 1;
            Debug.Log($"mother: {motherClip.length} childlength: {clip.length} delay: {delay} childEndTime: {childEndTime} mother/clip: {childEndTime / motherClip.length} floor: {Mathf.FloorToInt(childEndTime / motherClip.length)} playEvery: {playEvery}");

            var newChild = new ChildLoop
            {
                Clip = clip,
                PlayEvery = playEvery,
                Source = source,
                Delay = delay,
            };
            ChildLoops.Add(newChild);
        }
    }
    
    private void PlayChildLoops()
    {
        motherAudioSource.Play();

        for (int i=0; i<ChildLoops.Count; i++)
        {
            var loop = ChildLoops[i];
            if (loop.LoopCounter % loop.PlayEvery == 0)
            {
                loop.Source.loop = false;
                loop.Source.clip = loop.Clip;
                loop.Source.PlayDelayed(loop.Delay - DelayMod);
            }
            loop.LoopCounter++;
            ChildLoops[i] = loop;
        }
    }


    private void LateUpdate()
    {
        if (motherClip!=null && !motherAudioSource.isPlaying)
        {
            PlayChildLoops();
        }
    }

}
