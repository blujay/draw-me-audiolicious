using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class GrabbableHolon : OVRGrabbable
{

    public UnityEvent onGrab;
    public UnityEvent onRelease;

    protected override void Start()
    {
        base.Start();
    }

    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        base.GrabBegin(hand, grabPoint);
        Invoke("startGrab", 0f);
        var customHand = hand as HolonGrabber;
        if (customHand != null)
        {
            OVRInput.Controller currentHand = customHand.ReturnHand();
        }
    }


    override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        base.GrabEnd(linearVelocity, angularVelocity);
        onRelease.Invoke();
    }

    private void startGrab()
    {
        Debug.Log("startGrab was called");
        onGrab.Invoke();
    }
}
