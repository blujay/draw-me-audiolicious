using System;
using System.Collections;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEditor;
using UnityEngine;

// Clone audioreactive object
//
namespace Networking.Pun2

{

    public class ClonePrefabAudioVisualizer : MonoBehaviourPun
    {
        // [SerializeField] enum Hand { Right, Left };
        // [SerializeField] Hand hand;
        public bool pressedLastFrame = false;
        public bool clonedLastFrame = false;
        private string myPrefabName;
        private Vector3 myScale;
        private Transform thisObjectTransform;
        [NonSerialized] public OVRGrabber grabber;
        [NonSerialized] public musicBarScript myMusicBarObject;
        [NonSerialized] public MusicVisualizer myVisualizer;
        [NonSerialized] public Band band;

        IEnumerator StartClone()
        {
            {
                Clone();
                Debug.Log("cloned prefab");
                yield return clonedLastFrame = true;
            }
        }

        //public GameObject prefabToClone;

        private void Start()
        {
            thisObjectTransform = gameObject.transform;
            myPrefabName = gameObject.name;
            myMusicBarObject = gameObject.GetComponent<musicBarScript>();
            myVisualizer = gameObject.GetComponentInParent<MusicVisualizer>();
        }

        void Update()
        {
            myScale = transform.localScale;
            float triggerAmount = 0;

            if (photonView.IsMine)
            {

                triggerAmount = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

                if (triggerAmount > 0.75f)
                {
                    pressedLastFrame = true;

                    if (pressedLastFrame && !clonedLastFrame && GetComponent<PunOVRGrabbable>().isGrabbed)
                    {
                        StartCoroutine("StartClone", 1f);
                        return;
                    }
                }

                bool releasedTrigger = triggerAmount < 0.25f && pressedLastFrame;

                if (releasedTrigger)
                {
                    Debug.Log("trigger released");
                    pressedLastFrame = false;
                    clonedLastFrame = false;

                }

            }
        }
            public void Clone()
        {
            myScale = transform.localScale;
            GameObject obj = PhotonNetwork.Instantiate(myPrefabName, this.gameObject.transform.position, this.gameObject.transform.rotation, 0);
            obj.transform.parent = this.thisObjectTransform.parent;
            obj.gameObject.name = myPrefabName;
            obj.GetComponent<musicBarScript>().Visualizer = GetComponentInParent<MusicVisualizer>();
            obj.GetComponent<musicBarScript>().band = thisObjectTransform.GetComponent<musicBarScript>().band;

            // Prevent cloned objects colliding with original.
            // This assumes that all gameobjects in the clonable's hierarchy are on the default layer
            // We undo this in Hand.DetachObject()
            // Hacky! We probably need to keep track of the entire "isTrigger" status for the cloned
            // objects hierarchy but that seems like a lot of effort.
            obj.layer = LayerMask.NameToLayer("Cloning");
            foreach (var go in obj.GetComponentsInChildren<Transform>())
            {
                go.gameObject.layer = LayerMask.NameToLayer("Cloning");
            }
            //hand.AttachObject(clone, hand.GetBestGrabbingType(GrabTypes.None), Hand.AttachmentFlags.ParentToHand);
        }
    }
}

