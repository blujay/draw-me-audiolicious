using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrapSnap : MonoBehaviour
{
    // Start is called before the first frame update

    public int snapRotationAngle = 45;
    void Start()
    {
        
    }

    static float RoundTo(float val, int snapRotationAngle)
    {
        val /= snapRotationAngle;
        val = Mathf.Round(val);
        val *= snapRotationAngle;
        return val;
    }

    // Update is called once per frame
    void Update()
    {
        var rot = transform.rotation;
        var angles = rot.eulerAngles;
        angles = new Vector3(RoundTo(angles.x, snapRotationAngle), RoundTo(angles.y, snapRotationAngle), RoundTo(angles.z, snapRotationAngle));
        rot.eulerAngles = angles;
        transform.rotation = rot;
    }
}
