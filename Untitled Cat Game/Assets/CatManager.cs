using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CatManager : MonoBehaviour
{
    public bool followingPath;
    public bool sit;

    public CinemachineDollyCart dollyCart;
    public CinemachineSmoothPath beginningTrack;
    public Transform parentObject;

    public Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        parentObject.parent = dollyCart.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (followingPath)
        {
            parentObject.position = dollyCart.transform.position;
            parentObject.rotation = dollyCart.transform.rotation;
        }
        if(dollyCart.m_Position == beginningTrack.PathLength)
        {
            followingPath = false;
            parentObject.parent = null;
        }
    }
}
