using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPM : MonoBehaviour
{
    public static BPM instance;
    public float _bpm;
    private float _beatInterval, _beatIntervalD16;
    private float _beatTimer, _beatTimerD16;
    public static bool _beatFull, _beatD16;
    public static int _beatCountFull, _beatCountD16;
    bool started;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        StartBPM();
    }

    public void StartBPM()
    {
        started = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!started) return;
        BeatDetection();
    }

    void BeatDetection()
    {
        //full beat count
        _beatFull = false;
        _beatInterval = 60 / _bpm;
        _beatTimer += Time.deltaTime;
        if(_beatTimer >= _beatInterval)
        {
            _beatTimer -= _beatInterval;
            _beatFull = true;
            _beatCountFull++;
            Debug.Log("FullBeat");
        }
        //divided beat count
        _beatD16 = false;
        _beatIntervalD16 = _beatInterval / 16;
        _beatTimerD16 += Time.deltaTime;
        if(_beatTimerD16 >= _beatIntervalD16)
        {
            _beatTimerD16 -= _beatIntervalD16;
            _beatD16 = true;
            _beatCountD16++;
            //Debug.Log("D16");
        }
    }
}
