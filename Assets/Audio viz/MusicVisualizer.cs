using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MusicVisualizer : MonoBehaviour {

    public int numberOfBars = 32;
    public int numberOfSamples = 512;

    public GameObject barObject;
    public AudioSource musicSource;
    public float barSpacing;
    public FFTWindow fftWindow;
    public float baseBufferDecrease;
    public float bufferMultiplier;
    public float audioProfile;

    private string barObjectName;
    public Color barColor;


    float[] samples;
    public Band[] bands;

    // Use this for initialization
    void Start () {
        samples = new float[numberOfSamples];
        bands = new Band[numberOfBars];
        barObjectName = barObject.name;
        for (int i = 0; i < bands.Length; i++)
        {
            bands[i] = new Band(0,0,0,barColor);
        }

        float boxGenStartPointX = -(barSpacing * numberOfBars) / 2;

        for (int i = 0; i < numberOfBars; i++)
        {
            
            GameObject g = PhotonNetwork.Instantiate(barObject.name, transform.position, transform.rotation);
            g.transform.localPosition = new Vector3(transform.position.x + barSpacing * i, transform.position.y, transform.position.z);
            g.GetComponent<musicBarScript>().band = i;
            g.GetComponent<musicBarScript>().Visualizer = this;
            g.GetComponent<musicBarScript>().barColor = barColor;
            g.transform.parent = this.transform;
            g.gameObject.name = barObjectName;

        }

        applyAudioProfile(audioProfile);
        //musicSource.time = 80;
	}

    void applyAudioProfile(float value)
    {
        foreach(Band b in bands){
            b.freqBandHighest = value;
        }
    }
	
	// Update is called once per frame
	void Update () {
        GetSpectrumData();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        ChangeColor();
    }


    void GetSpectrumData()
    {
        musicSource.GetSpectrumData(samples, 0, fftWindow);
    }

    void MakeFrequencyBands()
    {
        int count = 0;
        int sampleCount = 1;
        int power = 0;

        for(int i = 0; i < numberOfBars; i++)
        {
            float average = 0;

            if (i == 8 || i == 16 || i == 20 || i == 24 || i == 28)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power);
                if (power == 3)
                {
                    sampleCount -= 2;
                }
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += (samples[count] * (count + 1));
                count++;
            }

            average /= count;
            bands[i].freqband = average * 80;
        }
        
    }

    void BandBuffer()
    {
        for (int i = 0; i < numberOfBars; ++i)
        {
            if (bands[i].freqband > bands[i].bandBuffer)
            {
                bands[i].bandBuffer = bands[i].freqband;
                bands[i].bufferDecrease = baseBufferDecrease;
            }
            else if (bands[i].freqband < bands[i].bandBuffer)
            {
                bands[i].bandBuffer -= bands[i].bufferDecrease;
                bands[i].bufferDecrease *= bufferMultiplier;
            }
        }
    }

    void CreateAudioBands()
    {
        for (int i = 0; i < numberOfBars; i++)
        {
            Band b = bands[i];
            if (b.freqband > b.freqBandHighest)
            {
                b.freqBandHighest = b.freqband;
            }
            b.audioBand = (b.freqband / b.freqBandHighest);
            b.audioBandBuffer = (b.bandBuffer / b.freqBandHighest);
            
        }
    }

    void ChangeColor()
    {
        for (int i = 0; i < numberOfBars; i++)
        {
            Band b = bands[i];
            b.barColor.r = b.freqband + b.bandBuffer;
            b.barColor.g = b.audioBand + b.audioBandBuffer;
            b.barColor.b = (b.bandBuffer * b.freqband) /255;
            b.barColor = Color.Lerp(b.barColor, b.barColor, 0.4f);

        }

    }
}

[System.Serializable]
public class Band
{
    public float freqband = 0, bandBuffer = 0, bufferDecrease = 0, audioBand = 0, audioBandBuffer = 0, freqBandHighest = 0;
    public Color barColor;
    public float red = 0, green = 0, blue = 0; 

    public Band(float frequency, float buffer, float bufferDecrease, Color barColor)
    {
        this.freqband = frequency;
        this.bandBuffer = buffer;
        this.bufferDecrease = bufferDecrease;
        this.red = barColor.r;
        this.green = barColor.g;
        this.blue = barColor.b;
        this.barColor = new Color(barColor.r, barColor.g, barColor.b);
    }
}