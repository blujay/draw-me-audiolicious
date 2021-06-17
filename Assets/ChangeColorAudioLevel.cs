using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeColorAudioLevel : MonoBehaviour

{

    [SerializeField] Lasp.AudioLevelTracker _input = null;
    public float inputLevel;
    public Transform prefab;
    //public Transform spawnPoint;
    public Color newColor;
    public Material lowMaterial;
    public Material midMaterial;
    public Material highMaterial;
    private bool reachedThreshold;
    // Start is called before the first frame update
    void Start()
    {
        inputLevel = _input.inputLevel;
    }

    // Update is called once per frame
    void Update()
    {
        var slice = _input.audioDataSlice;
        inputLevel = _input.inputLevel;
        if (_input.inputLevel < -35)
        {
            GetComponent<MeshRenderer>().material = lowMaterial;
        }
        else if (_input.inputLevel >= -30 && _input.inputLevel <= -20)
        {
            //GameObject.Instantiate(prefab, transform.position, Quaternion.identity);
            GetComponent<MeshRenderer>().material = midMaterial;
        }
        else GetComponent<MeshRenderer>().material = highMaterial;
           
    }

}
