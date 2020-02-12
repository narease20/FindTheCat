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
    [HideInInspector]
    public float throwPower;
    public bool sizeDown;

    public virtual void Update()
    {
        switch (weight)
        {
            case 0:
                throwPower = 2f;
                break;
            case 1:
                throwPower = 1.5f;
                break;
            case 2:
                throwPower = 1f;
                break;
            case 3:
                throwPower = 0.75f;
                break;
            case 4:
                throwPower = 0.5f;
                break;
            case 5:
                throwPower = 0.25f;
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
