using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform mTarget;

    [SerializeField, Range(0.1f, 10f)]
    public float mSpeed = 5.0f;

    public bool following = true, focusedWall = false;
    //public SphereCollider burnRange;
    [SerializeField, Range(0.1f, 10f)]
    public float burnTimer = 5f, followWaitTimer = 1f;
    [SerializeField, Range(0.1f, 10f)]
    public float minBurnDistance = 1f, maxBurnDistance = 5f;
    public FireWall closestFireWall;
    public FireWall affectedFireWall;

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
        BushFinder();
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

    public void BushFinder()
    {
        affectedFireWall = ClosestFireWall();
        if (affectedFireWall)
        {
            following = false;
            StopAllCoroutines();
            StartCoroutine(WallWait());
            BushDestroyer();
            return;
        }
    }

    IEnumerator WallWait()
    {
        focusedWall = true;
        transform.LookAt(new Vector3(closestFireWall.transform.position.x, 0, closestFireWall.transform.position.z), Vector3.up);
        yield return new WaitForSeconds(burnTimer);
    }

    void BushDestroyer()
    {
        if (affectedFireWall && affectedFireWall.canBeBurned)
        {
            affectedFireWall.DestroyMe();
            focusedWall = false;
            closestFireWall = null;
            affectedFireWall = null;
            StartCoroutine(TargetTimer());
        }
    }

    private FireWall ClosestFireWall()
    {
        FireWall nearest = null;
        float distance;

        if (closestFireWall != null)
        {
            distance = Vector3.Distance(transform.position, closestFireWall.transform.position);
            
            if(distance >= minBurnDistance && distance <= maxBurnDistance)
            {
                nearest = closestFireWall;
            }

            return nearest;
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<FireWall>())
        {
            closestFireWall = other.gameObject.GetComponent<FireWall>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (closestFireWall)
        {
            closestFireWall = null;
        }
    }
    
    IEnumerator TargetTimer()
    {
        yield return new WaitForSeconds(followWaitTimer);
        following = true;
    }
}
