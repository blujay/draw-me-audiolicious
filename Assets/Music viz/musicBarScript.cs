using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicBarScript : MonoBehaviour {

    
    public float minScale;
    public float scaleMultiplier;

    [NonSerialized] public int band;
    [NonSerialized] public MusicVisualizer Visualizer;

	// Use this for initialization
	void Start () {
    }

    // Update is called once per frame
    void Update() {
        transform.localScale = new Vector3(transform.localScale.x, Visualizer.bands[band].bandBuffer * scaleMultiplier + minScale, transform.localScale.z);
	}
}
