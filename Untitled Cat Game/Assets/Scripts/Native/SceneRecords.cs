using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRecords : MonoBehaviour
{
    public Animator transition;

    [SerializeField, Range(1, 3)]
    public float transitionTime = 1f;

    //Probably want to put some sort of player data management here

    // Update is called once per frame
    void Update()
    {
        
    }
}
