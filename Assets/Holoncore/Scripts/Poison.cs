using Networking.Pun2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//Attach this script to an object that you an empty game object and call it 'glue' 
//Any object that it then touches becomes sticky


public class Poison : MonoBehaviour
{

    void Start()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PunOVRGrabbable>() != null && other.gameObject.isStatic == false)
        Destroy(other.gameObject);
    }

}