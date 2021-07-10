using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HolonGrabEvents : OVRGrabbable
{

    public UnityEvent onGrab;
    public UnityEvent onRelease;

    private Transform _parent;

    protected override void Start()
    {
        base.Start();
        _parent = transform.parent;
    }

    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        Invoke("startGrab", 1f * Time.deltaTime);
        var customHand = hand as HolonGrabber;
        if (customHand != null)
        {
            OVRInput.Controller currentHand = customHand.ReturnHand();
            
        }
    }


    override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        Invoke("reparent", 1f * Time.deltaTime);
        onRelease.Invoke();
    }

    private void startGrab()
    {
        Debug.Log("startGrab was called");
        onGrab.Invoke();
    }

    private void reparent()
    {
        Debug.Log("reparent was called");
        transform.parent = _parent;
        
    }
}
