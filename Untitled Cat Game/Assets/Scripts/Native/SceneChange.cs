using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MeshCollider))]
public class SceneChange : MonoBehaviour
{
    [Header("Scene Management")]

    [Tooltip("The scene you want the player to be sent to.")]
    public string newScene;

    [Tooltip("Does this collider have the ability to transport the player?")]
    public bool canTransferScenes;

    [HideInInspector]
    public MeshCollider cc;
    [HideInInspector]
    public SceneRecords recorder;

    private void Start()
    {
        cc = GetComponent<MeshCollider>();
        cc.convex = true;
        cc.isTrigger = true;

        recorder = FindObjectOfType<SceneRecords>();

        //recorder = GetComponent<SceneRecords>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canTransferScenes)
        {
            //SceneManager.LoadScene(newScene);
            StopAllCoroutines();
            StartCoroutine(LoadLevel());
        }
    }

    // Plays the animation located on SceneRecords while transitioning between scenes
    IEnumerator LoadLevel()
    {
        recorder.transition.SetTrigger("Start");

        yield return new WaitForSeconds(recorder.transitionTime);

        SceneManager.LoadScene(newScene);
    }



    // Activate and Deactivate the "trigger" so a data manager can handle progression
    public void ActivateTrigger()
    {
        canTransferScenes = true;
    }
    public void DeactivateTrigger()
    {
        canTransferScenes = false;
    }
}
