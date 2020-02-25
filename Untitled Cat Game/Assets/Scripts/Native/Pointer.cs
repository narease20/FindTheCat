using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Pointer : MonoBehaviour
{
    public SteamVR_Action_Boolean interactUI = null;

    public float defaultLength = 4.0f;
    public GameObject dot;
    private Renderer dotRenderer;
    public VRInputModule inputModule;

    public Material disable;
    public Material enable;

    //private LineRenderer lineRenderer = null;

    private void Awake()
    {
        //lineRenderer = GetComponent<LineRenderer>();
        dotRenderer = dot.GetComponent<Renderer>();
        dotRenderer.material = disable;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();
        if (interactUI.GetStateDown(SteamVR_Input_Sources.Any))
        {
            TriggerPressed();
        }
    }

    private void UpdateLine()
    {
        // Raycast
        RaycastHit hit = CreateRaycast(defaultLength);

        // Default end
        Vector3 endPosition = transform.position + (transform.forward * defaultLength);

        // Based on hit
        if(hit.collider != null)
        {
            endPosition = hit.point;
            if(hit.collider.tag == "Button")
            {
                dotRenderer.material = enable;
            }
        }

        // Displays the dot's enabled texture sometimes after colliding with the button. Might want to check the tag with an or statement here
        if(hit.collider == null)
        {
            dotRenderer.material = disable;
        }

        // Set postion of dot
        dot.transform.position = endPosition;

        // Set positions of line re
        //lineRenderer.SetPosition(0, transform.position);
        //lineRenderer.SetPosition(1, endPosition);

    }

    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLength);

        return hit;
    }

    private void TriggerPressed()
    {
        RaycastHit hit;
        
        if(Physics.Raycast(transform.position, transform.forward, out hit, defaultLength))
        {
            if (hit.collider.tag == "Button")
            {
                //ButtonInterface collision = hit.collider.GetComponent<ButtonInterface>();
                //collision.ActivateButton();
                hit.collider.GetComponent<ButtonInterface>().ActivateButton();

                Debug.Log("Clicked Something??");
            }
            Debug.DrawRay(transform.position, transform.forward, Color.cyan, hit.distance);
            //Debug.Log("Found something" + hit.collider.tag);
        }
    }

}
