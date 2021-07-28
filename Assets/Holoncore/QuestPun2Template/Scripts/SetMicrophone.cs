using Photon.Pun;
using UnityEngine;
using Photon.Voice.Unity;

namespace Networking.Pun2
{
    public class SetMicrophone : MonoBehaviourPun
    {
        //For making sure that microphone is found and set to "Recorder" component from Photon Voice
        
        private Recorder recorder;
        public VoiceConnection voiceConnection;

        private void Start()
        {
            
            string[] devices = Microphone.devices;
            voiceConnection = FindObjectOfType<VoiceConnection>();
            if (devices.Length > 0)
            {
                recorder = GetComponent<Recorder>();
                voiceConnection.InitRecorder(recorder);
                recorder.Init(voiceConnection);
                recorder.UnityMicrophoneDevice = devices[0];
                
            }
        }

    }
}
