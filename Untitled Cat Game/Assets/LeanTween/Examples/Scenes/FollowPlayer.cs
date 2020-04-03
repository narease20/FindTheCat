using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform mTarget;

    [SerializeField, Range(0.1f, 10f)]
    public float mSpeed = 5.0f;

    public bool following = true;
    public SphereCollider burnRange;
    [SerializeField, Range(0.1f, 10f)]
    public float burnTimer;
    public FireWall closestFireWall;

    // make a constant for wrath to stop at a certain distance
    public float EPSILON = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (following)
        {
            FollowTarget();
        }

    }

    public void FollowTarget()
    {
        /* LookAt makes Wrath look at the player. 
        * May need to erase this part because it will probably rotate his whole body. */
        transform.LookAt(new Vector3 (mTarget.position.x, 0, mTarget.position.z), Vector3.up);
        //zRot = Mathf.Clamp()


        //    Vector3 difference = mTarget.position - transform.position;
        //    float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        //   float rotationX = Mathf.Atan2(difference.z, difference.y) * Mathf.Rad2Deg;
        //  transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

        /* this part should make wrath follow position. */
        if ((transform.position - mTarget.position).magnitude > EPSILON)
            transform.Translate(0.0f, 0.0f, mSpeed * Time.deltaTime);
    }

    /* Doesnt Work
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FireWall")
        {
            closestFireWall = other.GetComponent<FireWall>();
            Debug.Log("Got a wall");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (closestFireWall && !(other.gameObject.tag == "FireWall"))
        {
            closestFireWall = null;
            Debug.Log("removed a wall");
        }
    }
    */

    public void BushFinder()
    {
        following = false;
        // Check around for bush then call its function to destory it.
        StopAllCoroutines();
        StartCoroutine(WallWait());
        if (closestFireWall)
        {
            closestFireWall.DestroyMe();
        }

        following = true;
    }

    IEnumerator WallWait()
    {
        transform.LookAt(new Vector3(closestFireWall.transform.position.x, 0, closestFireWall.transform.position.z), Vector3.up);
        yield return new WaitForSeconds(burnTimer);
        //transform.LookAt(new Vector3(closestFireWall.transform.position.x, 0, closestFireWall.transform.position.z), Vector3.up);
    }
}
