using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class InteractableLightSource : Interactable
{
    Light lightSource;
    public float lightRange = 6;
    public float lightIntensity = 2;

    [Tooltip("How much to multiply the range and intensity by to shine more light"), Range(.05f, 5f)]
    public float powerMultiply = 2;
    [Tooltip("How long does the light last"), Range(.05f, 5f)]
    public float powerTime = 2;
    public float poweredLightRange;
    public float poweredLightIntensity;
    bool currentlyPowered = false;

    private void Start()
    {
        lightSource = GetComponent<Light>();
        lightRange = lightSource.range;
        lightIntensity = lightSource.intensity;

        poweredLightRange = lightRange * powerMultiply;
        poweredLightIntensity = lightIntensity * powerMultiply;
    }

    public void Update()
    {
        /*
        if (currentlyPowered)
        {
            lightSource.range = poweredLightRange;
            lightSource.intensity = poweredLightIntensity;
        }
        if (!currentlyPowered)
        {
            lightSource.range = lightRange;
            lightSource.intensity = lightIntensity;
        }

        
        if (currentlyPowered && lightSource.range < poweredLightRange && lightSource.intensity < poweredLightIntensity)
        {
            lightSource.range += Time.deltaTime;
            lightSource.intensity += Time.deltaTime;
        }
        if (!currentlyPowered && lightSource.range < lightRange && lightSource.intensity < lightIntensity)
        {
            lightSource.range -= Time.deltaTime;
            lightSource.intensity -= Time.deltaTime;
        }
        */
    }

    public override void Action()
    {
        //base.Action();
        LightTimer();
    }

    //Doesnt work. Just edit the shader's power instead
    IEnumerator LightTimer()
    {
        lightSource.range = poweredLightRange;
        lightSource.intensity = poweredLightIntensity;
        //currentlyPowered = true;
        yield return new WaitForSeconds(powerTime);
        //currentlyPowered = false;
        lightSource.range = lightRange;
        lightSource.intensity = lightIntensity;
    }
}
