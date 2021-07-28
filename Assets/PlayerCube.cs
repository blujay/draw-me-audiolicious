using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;


public class PlayerCube : MonoBehaviour
{
    private Material cubeMaterial;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            Debug.Log(string.Format("ActorNumber {0}", GetComponent<PhotonView>().OwnerActorNr));
        }

        else
        {
            Debug.Log(string.Format("ActorNumber {0}", GetComponent<PhotonView>().OwnerActorNr));
        }
    }
}
