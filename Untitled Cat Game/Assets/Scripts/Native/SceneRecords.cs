using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRecords : MonoBehaviour
{
    [Header("Crystal Transitional Variables")]
    public Animator transition;

    [SerializeField, Range(1, 3)]
    public float transitionTime = 1f;

    public Material nonActiveCrystal;
    public Material activeCrystal;

    [Header("Specific Scene Objects")]
    public GameObject fog;
    [Tooltip("The name of the scene where the fog should be spawned")]
    public string nameSceneFog;
    [Range(0.5f, 10f), Tooltip("Distance from the player. Only takes affect when spawned but that can be fixed. Ranges from 0.5 to 10.")]
    public float distanceFromPlayer = 5;
    
    public GameObject areaSpecificObject;
    public Camera playerCam;
    Scene currentScene;

    //Probably want to put some sort of player data management here

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        if(currentScene.name == nameSceneFog)
        {
            areaSpecificObject = Instantiate(fog, playerCam.transform.position + (playerCam.transform.forward * distanceFromPlayer), playerCam.transform.rotation);
            areaSpecificObject.transform.Rotate(areaSpecificObject.transform.eulerAngles.x, 90, -90);
            areaSpecificObject.transform.parent = playerCam.transform;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ParticleMaker(ParticleSystem particleToMake, Transform placement, float timeBeforeDelete)
    {
        ParticleSystem temp = Instantiate(particleToMake, placement.position, placement.rotation);
        Destroy(temp, timeBeforeDelete);
        return;
    }
    public void ParticleMaker(GameObject particleToMake, Transform placement, float timeBeforeDelete)
    {
        GameObject temp = Instantiate(particleToMake, placement.position, placement.rotation);
        Destroy(temp, timeBeforeDelete);
        return;
    }
}
