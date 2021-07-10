using Networking.Pun2;
using Photon.Chat.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//Attach this script to an object that you an empty game object and call it 'glue' 
//Any object that it then touches becomes sticky


public class GluePot : MonoBehaviour
{
    public Color stickyColor;
    void Start()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if ((other.GetComponent("StickyBit") as StickyBit) != null)
        {
            if(other.tag != "stuck")
            {
                other.GetComponent<StickyBit>().enabled = true;
                other.tag = "sticky";
                other.GetComponentInChildren<MeshRenderer>().material.color = stickyColor;
            }
           

            if ((other.GetComponent("ClonePrefab") as PunClonePrefab) != null)
            {
                other.GetComponent<PunClonePrefab>().enabled = false;
            }
        }
    }
    
}