using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEditor;
using UnityEngine;

//
//Creation example tool to instantiate a cube in the network using PhotonNetwork.Instantiate
//The cube ownership is set to actor number when created, and to its color using SetColor RPC
//
namespace Networking.Pun2
{

    public class ClonePrefabAudioVisualizer : MonoBehaviourPun
    {
       // [SerializeField] enum Hand { Right, Left };
       // [SerializeField] Hand hand;
        private bool pressedLastFrame = false;
        private bool clonedLastFrame = false;
        private string myPrefabName;
        private Transform thisObjectTransform;
        [NonSerialized] public OVRGrabber grabber;
        [NonSerialized] public musicBarScript myMusicBarObject;
        [NonSerialized] public MusicVisualizer Visualizer;
        [NonSerialized] public int band;

        //public GameObject prefabToClone;

        private void Start()
        {
            thisObjectTransform = gameObject.transform;
            myPrefabName = gameObject.name;
            myMusicBarObject = gameObject.GetComponent<musicBarScript>();
        }

        void Update()
        {
            
            float triggerAmount = 0;
            
            if (photonView.IsMine)
            {
                
                triggerAmount = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

                if (triggerAmount > 0.75f)
                {
                    pressedLastFrame = true;
                    
                    if (pressedLastFrame && !clonedLastFrame && grabber!=null)
                    {
                        Clone();
                        clonedLastFrame = true;
                    }
                }
                
                bool releasedTrigger = triggerAmount < 0.25f && pressedLastFrame;
                
                if(releasedTrigger)
                {
                    pressedLastFrame = false;
                    clonedLastFrame = false;

                }
                
            }
            
            void Clone()
            {
                GameObject g = PhotonNetwork.Instantiate(myPrefabName, this.gameObject.transform.position, this.gameObject.transform.rotation, 0);
                //obj.transform.position = transform.position;
                //obj.transform.rotation = transform.localRotation;
                g.transform.localScale = thisObjectTransform.transform.localScale;
                g.GetComponent<musicBarScript>().band = myMusicBarObject.band;
                //obj.GetComponent<ClonePrefab>().prefabToClone = prefabToClone;
                g.gameObject.name = "tomato";
                g.transform.parent = GetComponentInParent<MusicVisualizer>().transform;

                // Prevent cloned objects colliding with original.
                // This assumes that all gameobjects in the clonable's hierarchy are on the default layer
                // We undo this in Hand.DetachObject()
                // Hacky! We probably need to keep track of the entire "isTrigger" status for the cloned
                // objects hierarchy but that seems like a lot of effort.
                g.layer = LayerMask.NameToLayer("Cloning");
                foreach (var go in g.GetComponentsInChildren<Transform>())
                {
                    go.gameObject.layer = LayerMask.NameToLayer("Cloning");
                }

                //clone.transform.name = gameObject.name;
                //hand.AttachObject(clone, hand.GetBestGrabbingType(GrabTypes.None), Hand.AttachmentFlags.ParentToHand);
            }
               
        }

    }
}
