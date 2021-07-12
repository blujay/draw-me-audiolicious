using System;
using System.IO;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif



//
//Creation example tool to instantiate a cube in the network using PhotonNetwork.Instantiate
//The cube ownership is set to actor number when created, and to its color using SetColor RPC
//
namespace Networking.Pun2

{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(MakeAudioLoopObject))]

    public class MakeyLoop : MonoBehaviourPun
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
