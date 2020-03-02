using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCombiner))]
[RequireComponent(typeof(SphereCollider))]
public class ObjCombineT : Interactable
{
    public Interactable closeFriend;
    public GameObject newFriend;

    public bool FriendSet = false;

    // Start is called before the first frame update
    void Start()
    {
        SphereCollider sp = GetComponent<SphereCollider>();
        sp.isTrigger = true;
        sp.radius += transform.localScale.magnitude;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }

    public override void Action()
    {
        base.Action();

        newFriend = GetNearestInteractable();
        if (newFriend != null)
        {
            Hand holder = activeHand;
            activeHand = null;
            gameObject.GetComponent<MeshCombiner>().CallMe(newFriend);

            activeHand = holder;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Interactable>())
        {
            closeFriend = other.gameObject.GetComponent<Interactable>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Interactable>())
        {
            closeFriend = null;
        }
    }

    private GameObject GetNearestInteractable()
    {
        GameObject nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        if (closeFriend != null)
        {
            distance = (closeFriend.transform.position - transform.position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = closeFriend.gameObject;
            }

            return nearest;
        }

        return null;
    }
}
