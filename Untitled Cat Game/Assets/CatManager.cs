using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


/* This block of comments is because it's 2am and I don't know how to code. 
 * First, make a checker for if the cat has been pet. 
 * put the whole dolly cart movement system inside that check.
 * If the cat has been pet, THEN it starts to follow the path. 
 * But it needs to trigger a couple of animations first?? like, sit->stand->walk.
 * THose could be on a short timer, I guess.
 * I'm sorry. I tried to help. I failed. I need sleep. 
 * /
 */

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
            if (anim.GetBool("isSitting"))
            {
                anim.SetBool("isSitting", false);
                anim.SetBool("isWalking", true);
            }

            parentObject.position = dollyCart.transform.position;
            parentObject.rotation = dollyCart.transform.rotation;
        }
        if(dollyCart.m_Position == beginningTrack.MaxPos)
        {
            followingPath = false;
            parentObject.parent = null;
            anim.SetBool("isSitting", true);
        }
    }
}
