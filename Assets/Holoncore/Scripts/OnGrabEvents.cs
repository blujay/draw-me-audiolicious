using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnGrabEvents : MonoBehaviour
{

    public UnityEvent onGrab;
    public UnityEvent onRelease;

    // Start is called before the first frame update
    public void GrabBegin()
    {
        if (onGrab != null)
            onGrab.Invoke();
    }

    public void GrabEnd()
    {
        if (onRelease != null)
            onRelease.Invoke();
    }
}
