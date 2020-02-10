using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public Vector3 offset = Vector3.zero;
    public int weight = 0;
    [HideInInspector]
    public Hand activeHand = null;
    public float throwPower;

    public virtual void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        switch (weight)
        {
            case 0:
                rb.drag = 0;
                rb.angularDrag = 0.05f;
                break;
            case 1:
                rb.drag = 0.5f;
                rb.angularDrag = 0.04f;
                break;
            case 2:
                rb.drag = 0.04f;
                rb.angularDrag = 0.03f;
                break;
            case 3:
                rb.drag = 0.03f;
                rb.angularDrag = 0.02f;
                break;
            case 4:
                rb.drag = 0.02f;
                rb.angularDrag = 0.01f;
                break;
            case 5:
                rb.drag = 0.01f;
                rb.angularDrag = 0.005f;
                break;
            case 6:
                rb.drag = 50;
                rb.angularDrag = 0.001f;
                break;

        }
    }

    public virtual void Action()
    {
        print("Action");
    }

    public void ApplyOffset(Transform hand)
    {
        transform.SetParent(hand);
        transform.localRotation = Quaternion.identity;
        transform.localPosition = offset;
        transform.SetParent(null);
    }
}
