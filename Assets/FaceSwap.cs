using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(OVRGrabbable))]
public class FaceSwap : MonoBehaviourPun
{

    private void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        
        {
            if (other.GetComponent<OVRGrabbable>().isGrabbed && other.tag == "mask")
            {
                this.GetComponent<Renderer>().enabled = false;
                this.GetComponentInChildren<Renderer>().enabled = false;
                other.transform.rotation = this.transform.rotation;
                other.transform.position = this.transform.position;
                other.gameObject.transform.SetParent(this.transform);
            }

            if (other.GetComponent<OVRGrabbable>().isGrabbed && other.tag == "hair")
            {
                other.gameObject.transform.SetParent(this.transform);
                other.transform.rotation = this.transform.rotation;
                other.transform.position = this.transform.position;

            }
        }
    }
}
