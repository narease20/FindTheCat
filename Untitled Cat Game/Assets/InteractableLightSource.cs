using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class InteractableLightSource : Interactable
{
    public Light lightSource;
    public float lightRange = 6;
    public float lightIntensity = 2;

    [Tooltip("How much to multiply the range and intensity by to shine more light"), Range(.05f, 5f)]
    public float powerMultiply = 2;
    [Tooltip("How long does the light last"), Range(.05f, 5f)]
    public float powerTime = 2;
    float poweredLightRange;
    float poweredLightIntensity;
    bool currentlyPowered = false;

    public Material fog;
    public float fBase;
    public float fReduction;

    //float tempRange = 0;
    //float tempInten = 0;
    //float tempFRedux = 0;

    private void Start()
    {
        fBase = fog.GetFloat("_Strength");
        fReduction = fBase / powerMultiply;

        lightRange = lightSource.range;
        lightIntensity = lightSource.intensity;

        poweredLightRange = lightRange * powerMultiply;
        poweredLightIntensity = lightIntensity * powerMultiply;

        //Mathf.Clamp(tempRange, lightRange, poweredLightRange);
        //Mathf.Clamp(tempInten, lightIntensity, poweredLightIntensity);
    }

    public override void Update()
    { 
        /*
        if (currentlyPowered && tempRange < poweredLightRange && tempInten < poweredLightIntensity)
        {
            lightSource.range = tempRange += Time.deltaTime;
            lightSource.intensity = tempInten += Time.deltaTime;
            fReduction += Time.deltaTime;
            fog.SetFloat("_Strength", fReduction);
        }
        if (!currentlyPowered && tempRange > lightRange && tempInten > lightIntensity)
        {
            lightSource.range = tempRange -= Time.deltaTime;
            lightSource.intensity = tempInten -= Time.deltaTime;
            fReduction -= Time.deltaTime;
            fog.SetFloat("_Strength", fReduction);
        }
        */
    }


    public override void Action()
    {
        if (!currentlyPowered)
        {
            StartCoroutine(LightTimer());
        }
        return;
    }

    //Doesnt work. Just edit the shader's power instead
    IEnumerator LightTimer()
    {
        fog.SetFloat("_Strength", fReduction);
        lightSource.range = poweredLightRange;
        lightSource.intensity = poweredLightIntensity;
        currentlyPowered = true;
        yield return new WaitForSeconds(powerTime);
        currentlyPowered = false;
        lightSource.range = lightRange;
        lightSource.intensity = lightIntensity;
        fog.SetFloat("_Strength", fBase);
    }
}
