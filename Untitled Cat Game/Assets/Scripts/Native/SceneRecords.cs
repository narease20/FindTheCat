using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRecords : MonoBehaviour
{
    public Animator transition;

    [SerializeField, Range(1, 3)]
    public float transitionTime = 1f;

    public Material nonActiveCrystal;
    public Material activeCrystal;

    //Probably want to put some sort of player data management here

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ParticleMaker(ParticleSystem particleToMake, Transform placement, float timeBeforeDelete)
    {
        Instantiate(particleToMake, placement.position, placement.rotation);
        Destroy(particleToMake, timeBeforeDelete);
        return;
    }
    public void ParticleMaker(GameObject particleToMake, Transform placement, float timeBeforeDelete)
    {
        Instantiate(particleToMake, placement.position, placement.rotation);
        Destroy(particleToMake, timeBeforeDelete);
        return;
    }
}
