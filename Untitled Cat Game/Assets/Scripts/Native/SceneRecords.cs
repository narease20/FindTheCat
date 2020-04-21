using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

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

    public GameObject deadBush;
    Transform dbSpawn;

    [Header("VR vs FPS player spawning")]
    //Probably want to put some sort of player data management here
    public GameObject vrPlayer;
    public GameObject fpsPlayer;
    public GameObject player;
    [Range(10f, 600f), Tooltip("Seconds between checks for the headset")]
    public float timeBetweenChecks = 120f;

    public Transform startLocation;
    public Transform currentPosition;

    public bool checkedRecently = false;

    public float waitBeforeCam = 0.5f;
    
    private void Start()
    {
        currentPosition = startLocation;
        StartCoroutine(VRStart());

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
        if (player)
        {
            currentPosition = player.transform;
        }

        if (!checkedRecently)
        {

            //StopAllCoroutines();
            //StartCoroutine(VRCheck());
        }
        
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
    public void ParticleMaker(Transform placement, float timeBeforeDelete)
    {
        GameObject temp = Instantiate(deadBush, placement.position, deadBush.transform.rotation);
        Destroy(temp, timeBeforeDelete);
        return;
    }

    IEnumerator VRCheck()
    {
        checkedRecently = true;

        if (Valve.VR.OpenVR.IsHmdPresent() && player == fpsPlayer)
        {
            GameObject tempPlayer = player;
            player = Instantiate(vrPlayer, currentPosition.position, currentPosition.rotation);
            Destroy(tempPlayer);
            Debug.Log("HMD on");
            checkedRecently = true;
        }
        if (!Valve.VR.OpenVR.IsHmdPresent() && player == vrPlayer)
        {
            GameObject tempPlayer = player;
            player = Instantiate(fpsPlayer, currentPosition.position, currentPosition.rotation);
            Destroy(tempPlayer);
            Debug.Log("HMD off");
            checkedRecently = true;
        }

        yield return new WaitForSecondsRealtime(timeBetweenChecks);
        checkedRecently = false;
    }

    IEnumerator VRStart()
    {
        if (Valve.VR.OpenVR.IsHmdPresent())
        {
            player = Instantiate(vrPlayer, currentPosition.position, currentPosition.rotation);
            Transform temp = player.transform.GetChild(0);
            playerCam = temp.GetComponentInChildren<Camera>();
            Debug.Log("HMD on");
            checkedRecently = true;
        }
        if (!Valve.VR.OpenVR.IsHmdPresent())
        {
            player = Instantiate(fpsPlayer, currentPosition.position, currentPosition.rotation);
            playerCam = player.GetComponentInChildren<Camera>();
            Debug.Log("HMD off");
            checkedRecently = true;
        }
        checkedRecently = true;
        yield return new WaitForSecondsRealtime(timeBetweenChecks);
        checkedRecently = false;
    }
}
