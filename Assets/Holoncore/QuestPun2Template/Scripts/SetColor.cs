using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
//Sets the color of the first MeshRenderer/SkinnedMeshRenderer found with GetComponentInChildren
//
namespace Networking.Pun2
{
    public class SetColor : MonoBehaviourPun
    {
        //Color playerColor;
        Material playerMaterial;
        public Material foxHead;
        public Material badgerHead;
        public Material inkyHead;
        public Material catHead;
        public void SetColorRPC(int n)
        {
            GetComponent<PhotonView>().RPC("RPC_SetColor", RpcTarget.AllBuffered, n);
        }

        [PunRPC]
        void RPC_SetColor(int n)
        {
            switch (n)
            {
                case 1:
                    //playerColor = Color.red;
                    playerMaterial = foxHead;
                    break;
                case 2:
                    //playerColor = Color.cyan;
                    playerMaterial = badgerHead;
                    break;
                case 3:
                    //playerColor = Color.green;
                    playerMaterial = catHead;
                    break;
                case 4:
                    //playerColor = Color.yellow;
                    playerMaterial = badgerHead;
                    break;
                case 5:
                    //playerColor = Color.magenta;
                    playerMaterial = badgerHead;
                    break;
                default:
                    //playerColor = Color.black;
                    playerMaterial = badgerHead;
                    break;
            }
            // have removed colouring for now as using same badger face and hands. Might add back later
                //playerColor = Color.Lerp(Color.white, playerColor, 0.0f);
            if (GetComponentInChildren<MeshRenderer>() != null)
                playerMaterial = GetComponentInChildren<MeshRenderer>().material = playerMaterial;
            else if (GetComponentInChildren<SkinnedMeshRenderer>() != null)
                playerMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material = playerMaterial;
        }
    }
}
