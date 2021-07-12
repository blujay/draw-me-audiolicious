using System;
using UnityEditor;
using UnityEngine;
using OVR;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class CopyCat : MonoBehaviour
    {
        public bool pressedLastFrame = false;
        public bool clonedLastFrame = false;
        private GameObject prefabToClone;

    public void Awake()
    {
        pressedLastFrame = false;
        clonedLastFrame = false;
    }
    public void Start()
    {
        prefabToClone = this.gameObject;

    }
    void Update()

    {

           float triggerAmount = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);

        if (triggerAmount > 0.70f)
            {
                pressedLastFrame = true;

                if (pressedLastFrame && !clonedLastFrame && GetComponent<GrabbableHolon>().isGrabbed)
                {
                    clonedLastFrame = true;
                    Clone();
                }

            bool releasedTrigger = triggerAmount < 0.30f && pressedLastFrame;

            if (releasedTrigger)
            {
                pressedLastFrame = false;
                clonedLastFrame = false;
            }

        }
    }
    public void Clone()
    {
            
            prefabToClone = gameObject;
            GameObject obj = Instantiate(prefabToClone, prefabToClone.transform.position, prefabToClone.transform.rotation);
            obj.transform.localScale = prefabToClone.transform.localScale;
            obj.GetComponentInChildren<Renderer>().material.color = this.GetComponentInChildren<Renderer>().material.color;
            obj.gameObject.name = gameObject.name;
            obj.gameObject.GetComponent<AudioSource>().Play();
            obj.layer = LayerMask.NameToLayer("Cloning");
            foreach (var go in obj.GetComponentsInChildren<Transform>())
            {
                go.gameObject.layer = LayerMask.NameToLayer("Cloning");
            }
        }
    }
