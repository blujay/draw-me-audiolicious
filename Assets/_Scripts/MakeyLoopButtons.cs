using System;
using System.IO;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

namespace Networking.Pun2
{
    public class MakeyLoopButtons : MonoBehaviourPun
    {
        private bool pressedOnce = false;


        IEnumerator StartRecordingLoop()
        {
            yield return new WaitForSeconds(0.1f);
            yield return pressedOnce = true;
            GetComponent<MakeAudioLoopWithMother>().generated = false;
            GetComponent<MakeAudioLoopWithMother>().StartRecording();
            Debug.Log("Pressed once to turn ON");
        }

        IEnumerator StopRecordingLoop()
        {
            yield return new WaitForSeconds(0.1f);
            yield return pressedOnce = false;
            GetComponent<MakeAudioLoopWithMother>().StopRecording();
            Debug.Log("Pressed once to turn OFF");

        }

        void Update()
        {
            if (GetComponent<MakeAudioLoopWithMother>() != null && gameObject.GetComponent<PunOVRGrabbable>().isGrabbed)
            {
                if (OVRInput.GetDown(OVRInput.Button.One) && pressedOnce == false)
                {
                    StartCoroutine(StartRecordingLoop());
                    return;
                }

                if (OVRInput.GetUp(OVRInput.Button.One) && pressedOnce == true)
                {
                    StartCoroutine(StopRecordingLoop());
                    return;
                }
            }
            if(gameObject.GetComponent<PunOVRGrabbable>().isGrabbed == false && pressedOnce == true)
            {
                StartCoroutine(StopRecordingLoop());
                return;
            }
        }
    }
}
                
