using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    [Tooltip("Affects the proximity to the hand.")]
    public Vector3 offset = Vector3.zero;
    [SerializeField, Range(0, 6), Tooltip("Affects the power of the player's throw. Lower is easier to throw while higher is harder. 3 is the normal weight as it's between x2 power(0) and x0.25 power (6).")]
    public int weight = 0;

    [HideInInspector]
    public Hand activeHand = null;
    [HideInInspector]
    public float throwPower;

    [Header("Size Modification")]
    [Tooltip("Should this object be sized down when picked up?")]
    public bool sizeDown;
    [SerializeField, Range(0.1f, 0.95f), Tooltip("If it should be sized down, by how much.")]
    public float sizeDownAmount = 0.3f;

    public virtual void Update()
    {
        switch (weight)
        {
            case 0:
                throwPower = 2f;
                break;
            case 1:
                throwPower = 1.75f;
                break;
            case 2:
                throwPower = 1.5f;
                break;
            case 3:
                throwPower = 1f;
                break;
            case 4:
                throwPower = 0.75f;
                break;
            case 5:
                throwPower = 0.5f;
                break;
            case 6:
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
