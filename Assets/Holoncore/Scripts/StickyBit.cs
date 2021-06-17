using Networking.Pun2;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// if this sticky object touches another object
// The other object becomes a child of the sticky object
// The other object's grabbable collider becomes added to the grab points of the sticky object
// The grabbable component becomes disabled

public class StickyBit : MonoBehaviour
{

    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "sticky")
        {
            transform.SetParent(other.transform);
            gameObject.tag = "stuck";
            if ((other.GetComponent("ClonePrefab") as ClonePrefab) != null)
                GetComponent<ClonePrefab>().enabled = false;
        }
            this.enabled = false;
        }
    }

