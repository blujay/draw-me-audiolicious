using UnityEngine;

public class MotherloopManager : MonoBehaviour
{
    public struct ChildLoop
    {
        public float Duration;
        public float Delay;
        public int PlayEvery;
        public AudioSource audioSource;
    }

    public ChildLoop[] ChildLoops;
    public int LoopCounter;

    private ChildLoop currentChildLoop;
    private float motherloopDuration = 3;

    void Start()
    {
        InvokeRepeating(nameof(PlayMotherloop), 0, motherloopDuration);
    }
    
    private void PlayMotherloop()
    {
        foreach (var loop in ChildLoops)
        {
            if (LoopCounter % loop.PlayEvery == 0)
            {
                loop.audioSource.PlayDelayed(loop.Delay);
            }
        }
        LoopCounter++;
    }
}
