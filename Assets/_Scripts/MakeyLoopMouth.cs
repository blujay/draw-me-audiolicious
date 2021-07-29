using System;
using System.IO;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif



// make an audio loop

namespace Networking.Pun2

{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(MakeAudioLoopObject))]

    public class MakeyLoopMouth : MonoBehaviourPun
    {

        
        private void OnTriggerEnter(Collider other)
        {
            if(GetComponent<MakeAudioLoopObject>() != null && other.gameObject.tag == "mouth" && gameObject.GetComponent<PunOVRGrabbable>().isGrabbed)
            {
                GetComponent<MakeAudioLoopObject>().generated = false;
                GetComponent<MakeAudioLoopObject>().StartRecording();
            }
        }

        private void OnTriggerExit(Collider other)
        {   
            if(other.gameObject.tag == "mouth")
            {
                GetComponent<MakeAudioLoopObject>().StopRecording();
            }
        }

    }
}
