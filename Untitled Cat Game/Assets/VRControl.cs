using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRControl : MonoBehaviour
{
    private Transform head = null;
    public float maxHeadHeight = 2.5f;
    public float minHeadHeight = 0.3f;
    private CapsuleCollider player = null;

    private Camera vision = null;



    // Start is called before the first frame update
    void Start()
    {
        head = SteamVR_Render.Top().head;
        vision = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleHeight()
    {
        // Get heads local space
        float headHeight = Mathf.Clamp(head.localPosition.y, minHeadHeight, maxHeadHeight);
        //m_CharacterController.height = headHeight;
        //headCollision.center = new Vector3 (0, headHeight, 0);
        player.height = headHeight;

        // cut in half
        Vector3 newCenter = Vector3.zero;
        //newCenter.y = m_CharacterController.height / 2;
        //newCenter.y += m_CharacterController.skinWidth;

        //newCenter.y = headCollision.center.y / 2;
        //newCenter.y += headCollision.size.y;

        newCenter.y = player.height / 2;
        newCenter.y += player.center.y;

        // Move capsule in local space
        newCenter.x = head.localPosition.x;
        newCenter.z = head.localPosition.z;

        /*
        // rotate
        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;
        */


        // apply
        //m_CharacterController.center = newCenter;
        //m_CharacterController.height = headHeight / 1.3f;

        //groundCollision.center =  new Vector3(newCenter.x, newCenter.y - 0.3f, newCenter.z);
        //headCollision.center = new Vector3(newCenter.x, newCenter.y, newCenter.z);

        //PlayArea.transform.position = new Vector3(newCenter.x, newCenter.y - 0.3f, newCenter.z);

        player.center = new Vector3(newCenter.x, 0.3f, newCenter.z);
        player.height = headHeight;

        //Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y - 0.2f, Camera.transform.position.z);

        PlayArea.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
    }



}
