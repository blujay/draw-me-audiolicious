using Networking.Pun2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//Attach this script to an object that you an empty game object and call it 'glue' 
//Any object that it then touches becomes sticky


public class Solvent : MonoBehaviour
{

    void Start()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "stuck" || other.tag == "sticky")
        {
            other.GetComponent<StickyBit>().enabled = false;
            other.tag = "Untagged";
            other.transform.parent = null;
            other.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            if ((other.GetComponent("ClonePrefab") as ClonePrefab) != null)
            {
                other.GetComponent<ClonePrefab>().enabled = true;
            }
        }
    }

}