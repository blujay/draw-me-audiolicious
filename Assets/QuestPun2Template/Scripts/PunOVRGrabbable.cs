using UnityEngine;
using UnityEngine.Events;

//
//Custom grabbable script wich checks if the grabber "is mine" to actually grab
//

namespace Networking.Pun2
{
    [RequireComponent(typeof(Photon.Pun.PhotonView))]
    public class PunOVRGrabbable : OVRGrabbable
    {
        public UnityEvent onGrab;
        public UnityEvent onRelease;
        [SerializeField] bool hideHandOnGrab;
        Photon.Pun.PhotonView pv;
        Rigidbody rb;
        ClonePrefab clonePrefab;

        protected override void Start()
        {
            base.Start();
            pv = GetComponent<Photon.Pun.PhotonView>();
            rb = gameObject.GetComponent<Rigidbody>();
            clonePrefab = gameObject.GetComponent<ClonePrefab>();
        }

        override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
        {
            m_grabbedBy = hand;
            if (hand.GetComponent<Photon.Pun.PhotonView>().IsMine)
            {
                //Change ownership if the hand grabbing is mine
                GetComponent<Photon.Pun.PhotonView>().TransferOwnership(Photon.Pun.PhotonNetwork.LocalPlayer);
                m_grabbedCollider = grabPoint;
                pv.RPC("SetKinematicTrue", Photon.Pun.RpcTarget.AllBuffered); //changes the kinematic state of the object to all players when its grabbed
                if (clonePrefab != null)
                {
                    clonePrefab.grabber = m_grabbedBy;
                }
                if (onGrab != null)
                {
                    onGrab.Invoke();
                }
                if (hideHandOnGrab)
                    m_grabbedBy.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            }
        }

        override public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            //If the grabbed object is mine, release it
            if (gameObject.GetComponent<Photon.Pun.PhotonView>().IsMine)
            {
                rb.isKinematic = m_grabbedKinematic;
                pv.RPC("SetKinematicFalse", Photon.Pun.RpcTarget.AllBuffered);
                rb.velocity = linearVelocity;
                rb.angularVelocity = angularVelocity;
                if (hideHandOnGrab)
                    m_grabbedBy.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
                m_grabbedBy = null;
                m_grabbedCollider = null;
                if (clonePrefab != null)
                {
                    clonePrefab.grabber = null;
                }
                if (onRelease != null)
                    onRelease.Invoke();
            }
        }

        public new Collider[] grabPoints
        {
            get { return m_grabPoints; }
            set { grabPoints = value; }
        }

        virtual public void CustomGrabCollider(Collider[] collider)
        {
            m_grabPoints = collider;
        }

        [Photon.Pun.PunRPC]
        public void SetKinematicTrue()
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        [Photon.Pun.PunRPC]
        public void SetKinematicFalse()
        {
            rb.isKinematic = m_grabbedKinematic;
        }
    }
}