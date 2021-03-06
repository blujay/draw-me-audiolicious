using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float forwardForce;
    public float turnForce;

    [Range(0, 1)]
    public float driftFactor;

    Rigidbody rb;
    public Light leftRearLight;
    public Light rightRearLight;

    [HideInInspector]
    public float vAxis;
    [HideInInspector]
    public float hAxis;
    [HideInInspector]
    public float sidewaysVel;
    [HideInInspector]
    public float forwardVel;
    [HideInInspector]
    public float totalVel;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        vAxis = Input.GetAxis("Vertical");
        hAxis = Input.GetAxis("Horizontal");
        Vector3 sidewaysVector = SideWaysVelocity();
        Vector3 forwardsVector = ForwardVelocity();

        sidewaysVel = sidewaysVector.magnitude;
        forwardVel = forwardsVector.magnitude;
        rb.velocity = forwardsVector + sidewaysVector * driftFactor; //<-- This causes slow falling, but also makes the car more reactive
        totalVel = rb.velocity.magnitude;

        if (vAxis > 0)
        {
            rb.AddForce(transform.forward * forwardForce);
        }
        if (vAxis < 0)
        {
            leftRearLight.enabled = true;
            rightRearLight.enabled = true;
            rb.AddForce(transform.forward * -forwardForce);
        }
        else
        {
            leftRearLight.enabled = false;
            rightRearLight.enabled = false;
        }
      
        float tf = Mathf.Lerp(0, turnForce, rb.velocity.magnitude / 2);
        rb.angularVelocity = new Vector3(0, hAxis * tf, 0);
    }


    Vector3 ForwardVelocity()
    {
        return transform.forward * Vector3.Dot(rb.velocity, transform.forward);
    }

    Vector3 SideWaysVelocity()
    {
        return transform.right * Vector3.Dot(rb.velocity, transform.right);
    }

    
}