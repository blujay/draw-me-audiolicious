using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holonballs : MonoBehaviour
{
    // Experimental little co-routines that can be added to objects. Called as such because they accompany copy-pasta and are rather tasty. Too much can cause indigestion. 
    private Material holonMaterial;
    public Color changeColor;
    private Color holonStartColor;

    //make colour gameobject colour turn blue over 5 seconds
    public void Start()
    {
        holonMaterial = GetComponentInChildren<Renderer>().material;
        holonStartColor = holonMaterial.color;
        
    }

    IEnumerator ChangeHolonColor(Color startColor, Color endColor, float changeDuration)
    {
        float t = 0;

        while (t < changeDuration)
        {
            t += Time.deltaTime;
            holonMaterial.color = Color.Lerp(startColor, endColor, t / changeDuration);
            yield return null;
        }
    }

    public void changeHolonColor2()
    {
        StartCoroutine(ChangeHolonColor(holonStartColor, changeColor, 2));
    }

    public void revertColor2()
    {
        Color currentColor = holonMaterial.color;
        StartCoroutine(ChangeHolonColor(currentColor, holonStartColor, 2));
    }
}
