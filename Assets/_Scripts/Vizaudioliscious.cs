using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Maf'j is taking this script from a YouTube Tutorial by 'N3K EN' and renaming things 
// so that its more fun, more relevant to Inkibit and Holonspace.
// if you're a Unity developer, I understand your disdain at the aparent jumble of ideas... It's Digital Permaculture baby!!!.
// https://www.youtube.com/watch?v=wtXirrO-iNA&ab_channel=N3KEN

public class Vizaudioliscious : MonoBehaviour
{
    private const int SAMPLE_SIZE = 1024;

    public float rmsAverageOutputValue;
    public float dbLoudnessValue;
    public float pitchValue;

    public float jiggleModAmount = 50.0f;
    public float smoothModAmount = 10.0f;
    public float maxJigglyScale = 25.0f;
    public float keepPercentage = 0.5f;
    public float circleRadius = 2;
    public Transform prefabToMakeJiggle;
    private string prefabName;
    public Material jigglyMaterial;

    private AudioSource source;
    private float[] samples;
    private float[] spectrum;
    private float sampleRate;


    private Vector3 originPoint;

    public enum vizPresets
    {
        iridescentSeaShell,
        crazyColor,
        monochrome,
        playSchool,
        vividWildClarity,
        underwaterBlues,
        afterGlow,
        smoulderingEmbers,
        eightiesSkateRink,
        stainedGlassCaterpillar

    }

    public vizPresets vizPreset;

    // Start is called before the first frame update
    private void Start()
    {
        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;
        originPoint = this.transform.position;
        prefabName = prefabToMakeJiggle.gameObject.name;
        

        //MakeJiggliesRow();
        MakeJigglesCircle();
    }


    //make some sweet lil jigglies - objects that jiggle with the beat
    private Transform[] jigglyList; // a list to hold the jigglies in
    private float[] jigglyScale; // how big will they be - this holds the scale of each jiggly?
    public int amountOfJigglies = 10; // how many of them?

    private void MakeJiggliesRow()
    {
        jigglyList = new Transform[amountOfJigglies];
        jigglyScale = new float[amountOfJigglies];

        for (int i = 0; i < amountOfJigglies; i++)
        {
            GameObject jiggly;
            if(prefabToMakeJiggle)
            {
                jiggly = PhotonNetwork.Instantiate(prefabName, transform.position, transform.rotation);
                jiggly.gameObject.name = prefabToMakeJiggle.name;
            } else
            {
                jiggly = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            } 
            
            jigglyList[i] = jiggly.transform; // this sweet little jiggly is now in the list :)
            jigglyList[i].position = Vector3.right * i;
        }
    }

    private void MakeJigglesCircle()
    {
        jigglyScale = new float[amountOfJigglies];
        jigglyList = new Transform[amountOfJigglies];
        Vector3 center = Vector3.zero;

        //for some reason I can't get the directions right without displacing the parent object to the centre then moving back.. here goes
        transform.position = center;

        for (int i = 0; i < amountOfJigglies; i++)
        {
            GameObject jiggly;
            float angle = i * 1.0f / amountOfJigglies;
            angle = angle * Mathf.PI * 2;
            float x = center.x + Mathf.Cos(angle) * circleRadius;
            float y = center.y + Mathf.Sin(angle) * circleRadius;
            Vector3 jigglyPos = new Vector3(x, y, 0);

            if (prefabToMakeJiggle)
            {
                jiggly = PhotonNetwork.Instantiate(prefabName, transform.position, transform.rotation);
                jiggly.gameObject.name = prefabToMakeJiggle.name;
            }
            else
            {
                jiggly = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            }
            
            jiggly.GetComponentInChildren<Renderer>().material = jigglyMaterial;
            jiggly.transform.position = jigglyPos;
            jiggly.transform.parent = transform;
            jiggly.transform.rotation = Quaternion.LookRotation(Vector3.forward, jigglyPos);

            //now add to list of jigglies:
            jigglyList[i] = jiggly.transform;
        }
        //move it back to where it wanted to be in the first place... hmmm must be a neater way to do this.
        transform.position = originPoint;

    }

    // Update is called once per frame
    private void Update()
    {
        AnalyzeSound();
        JiggleJiggle();
        AddColors();
        
    }

    private void AnalyzeSound()
    {
        int i = 0;
        source.GetOutputData(samples, 0);

        //get RMS 'Average Output' values;
        float sum = 0;
        for(i = 0; i < SAMPLE_SIZE; i++)
        {
            sum = samples[i] * samples[i];
        }
        rmsAverageOutputValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        //get DB 'Loudness' value
        //Check out this page: https://answers.unity.com/questions/157940/getoutputdata-and-getspectrumdata-they-represent-t.html
        
        dbLoudnessValue = 20 * Mathf.Log10(rmsAverageOutputValue / 0.1f);

        //get sound spectrum

        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        //get pitch

        float maxValue = 0;
        var maxNumber = 0;
        
        for(i = 0; i < SAMPLE_SIZE; i++)
        {
            if (!(spectrum[i] > maxValue) || !(spectrum[i] > 0.0f))
                continue;

            maxValue = samples[i];
            maxNumber = i;
        }

        float frequencyNumber = maxNumber;
        
        if(maxNumber > 0 && maxNumber < SAMPLE_SIZE - 1)
        {
            var dLeft = spectrum[maxNumber - 1] / spectrum[maxNumber];
            var dRight = spectrum[maxNumber + 1] / spectrum[maxNumber];
            frequencyNumber += 0.5f * (dRight * dRight - dLeft * dLeft);
        }
        pitchValue = frequencyNumber * (sampleRate / 2) / SAMPLE_SIZE;
    }


//Make those jiggly babies dance in a row!!

private void JiggleJiggle()
    {
        int visualIndex = 0;
        int spectrumIndex = 0; //place in the spectrum where our single sample lives..ahhh sweet.
        int averageJigglySize = (int)((SAMPLE_SIZE * keepPercentage) / amountOfJigglies);
        // samples are divided up into the amount of jigglies you have.
        // so if we had 2 lil jigglies, then each would get 512 samples with our current default SAMPLE_SIZE of 1024

        while (visualIndex < amountOfJigglies)
        {
            int j = 0;
            float sum = 0;
            while (j < averageJigglySize)
            {

                sum += spectrum[spectrumIndex]; // had to google this.. += operator adds the value of the variable on the left with the value on the right
                spectrumIndex++; // move onto next sample in spectrum
                j++;
                // while J is smaller than 512 (assuming same example of 2 jigglies) so this loop runs 512 times!
                //next time around this loop - only 511 to go...
            }
            float jiggleHeight = sum * jiggleModAmount; // we're going to make our jiggly go up and down on Y axis... simple
            jigglyScale[visualIndex] -= Time.deltaTime * smoothModAmount; // This reduces the scale (visualIndex) over time..
            if (jigglyScale[visualIndex] < jiggleHeight)// if our jiggly goes below the starting point - is smaller than height
                jigglyScale[visualIndex] = jiggleHeight;// bump it back to where it should be then slowly fall down.

            if (jigglyScale[visualIndex] > maxJigglyScale)
                jigglyScale[visualIndex] = maxJigglyScale;

            jigglyList[visualIndex].localScale = Vector3.one + Vector3.up * jigglyScale[visualIndex];
            visualIndex++;
        }
    }

    private void OnValidate()
    {
        AddColors();
    }
    private void AddColors()
    {
        int visualIndex = 0;
        int spectrumIndex = 0; //place in the spectrum where our single sample lives..ahhh sweet.
        int averageJigglySize = (int)((SAMPLE_SIZE * keepPercentage) / amountOfJigglies);
        // samples are divided up into the amount of jigglies you have.
        // so if we had 2 lil jigglies, then each would get 512 samples with our current default SAMPLE_SIZE of 1024
        // each color - red, green, blue has max 255 value in each. So we have to do something with that...


        while (visualIndex < amountOfJigglies)
        {
            int j = 0;
            float sum = 0;
            while (j < averageJigglySize)
            {
                sum += spectrum[spectrumIndex]; // had to google this.. += operator adds the value of the variable on the left with the value on the right
                spectrumIndex++; // move onto next sample in spectrum
                j++;
                // while J is smaller than 512 (assuming same example of 2 jigglies) so this loop runs 512 times!
                //next time around this loop - only 511 to go...
            }
            float jiggleHeight = sum * jiggleModAmount; // we're going to use this to affect our values here
                                                        //make into switches - for now disable comments for the ones you want

            float red, green, blue;

            switch (vizPreset)
            {
                case vizPresets.iridescentSeaShell:
                        red = spectrum[spectrumIndex] + jiggleHeight * Mathf.Cos(visualIndex);
                        green = spectrum[spectrumIndex] + jiggleHeight * Mathf.Sin(visualIndex);
                        blue = spectrum[spectrumIndex] + jiggleHeight * Mathf.Atan(visualIndex);
                        break;

                case vizPresets.crazyColor:
                        red = Mathf.Cos(visualIndex + pitchValue)+ jiggleHeight; 
                        green = Mathf.Sin(visualIndex * pitchValue) + jiggleHeight; 
                        blue = Mathf.Cos(visualIndex/dbLoudnessValue + pitchValue);
                        break;

                case vizPresets.monochrome:
                        red = spectrum[spectrumIndex] + jiggleHeight;
                        green = spectrum[spectrumIndex] + jiggleHeight;
                        blue = spectrum[spectrumIndex] + jiggleHeight;
                        break;
                case vizPresets.playSchool:
                        red = spectrum[spectrumIndex] + jiggleHeight + Mathf.Cos(visualIndex);
                        green = spectrum[spectrumIndex] + jiggleHeight + Mathf.Sin(visualIndex);
                        blue = spectrum[spectrumIndex] + jiggleHeight + Mathf.Atan(visualIndex);
                        break;
                case vizPresets.vividWildClarity:
                        red = spectrum[spectrumIndex] + Mathf.Atan(jiggleHeight);
                        green = spectrum[spectrumIndex] + Mathf.Sin(jiggleHeight);
                        blue = spectrum[spectrumIndex] + Mathf.Tan(jiggleHeight);
                        break;
                case vizPresets.underwaterBlues:
                        red = Mathf.Log(dbLoudnessValue * pitchValue) + Mathf.Log(spectrumIndex);
                        green = spectrum[spectrumIndex] + Mathf.Sin(jiggleHeight);
                        blue = jiggleHeight * visualIndex - Mathf.Cos(rmsAverageOutputValue);
                        break;
                case vizPresets.afterGlow:
                        red = spectrum[spectrumIndex] * Mathf.Cos(pitchValue);
                        green = spectrum[spectrumIndex] * Mathf.Sin(pitchValue);
                        blue = spectrum[spectrumIndex] * Mathf.Tan(pitchValue);
                        break;
                case vizPresets.smoulderingEmbers:
                        red = spectrum[spectrumIndex] * Mathf.Cos(rmsAverageOutputValue*jiggleHeight)*2;
                        green = spectrum[spectrumIndex] * Mathf.Sin(rmsAverageOutputValue*jiggleHeight);
                        blue = spectrum[spectrumIndex] * Mathf.Atan(rmsAverageOutputValue*jiggleHeight);
                        break;
                case vizPresets.eightiesSkateRink:
                        red = spectrum[spectrumIndex] + pitchValue * Mathf.Cos(visualIndex);
                        green = spectrum[spectrumIndex] + pitchValue * Mathf.Sin(visualIndex);
                        blue = spectrum[spectrumIndex] + pitchValue * Mathf.Atan(visualIndex);
                        break;
                case vizPresets.stainedGlassCaterpillar:
                        red = spectrum[spectrumIndex] + jiggleHeight * Mathf.Cos(visualIndex);
                        green = spectrum[spectrumIndex] + jiggleHeight * Mathf.Sin(visualIndex);
                        blue = spectrum[spectrumIndex] + jiggleHeight * Mathf.Tan(visualIndex);
                        break;
                default:
                        red = spectrum[spectrumIndex];
                        green = spectrum[spectrumIndex];
                        blue = spectrum[spectrumIndex];
                        break;

                
            }

            Color newBarColor = new Color(red, green, blue);
            jigglyScale[visualIndex] -= Time.deltaTime * smoothModAmount; // This reduces the scale (visualIndex) over time..
            jigglyList[visualIndex].GetComponentInChildren<Renderer>().material.color = newBarColor;
            visualIndex ++;
        }
    }
}
