using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicBarScript : MonoBehaviour {

    public Color barColor;
    public Renderer vizMeshRenderer;
    public float minScale;
    public float scaleMultiplier;
    

    [NonSerialized] public int band;
    [NonSerialized] public MusicVisualizer Visualizer;

	// Use this for initialization
	void Start () {
        vizMeshRenderer.material.color = Visualizer.bands[band].barColor;
    }

    // Update is called once per frame
    void Update() {
        var scale = Visualizer.bands[band].bandBuffer * scaleMultiplier + minScale;
        transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
        //transform.localScale = new Vector3(transform.localScale.x, Visualizer.bands[band].bandBuffer * scaleMultiplier + minScale, transform.localScale.z);
        vizMeshRenderer.material.color = Visualizer.bands[band].barColor;
    }

}
