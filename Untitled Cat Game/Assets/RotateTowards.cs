using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    public Camera playerCam;
    public Transform player;

    public float turnRate = 3 * Mathf.PI;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 pos = transform.position + new Vector3(player.transform.rotation.x, 0, player.transform.rotation.z) + Vector3.forward;
        //Vector3 rot = new Vector3(playerCam.transform.rotation.x, 0, playerCam.transform.rotation.z);// + Vector3.up;

        //Vector3 target = Vector3.RotateTowards(transform.position, player.forward, 180, 0.1f);

        //Quaternion rot = Quaternion.LookRotation(playerCam.transform.forward, playerCam.transform.up);
        //Quaternion.RotateTowards(transform.rotation, playerCam.transform.rotation, 360);

        //transform.rotation = Quaternion.LookRotation(target, Vector3.up);
        //transform.LookAt(pos, rot);
        //transform.RotateAround();

        Vector3 targetDir = player.position - transform.position;

        targetDir.y = 0.0f;
        targetDir = targetDir.normalized;

        Vector3 currentDir = transform.forward;

        currentDir = Vector3.RotateTowards(currentDir, -targetDir, turnRate * Time.deltaTime, 4f);

        Quaternion qDir = new Quaternion();
        qDir.SetLookRotation(currentDir, Vector3.up);
        transform.rotation = qDir;

    }
}
