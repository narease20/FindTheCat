using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform mTarget;

    float mSpeed = 5.0f;

    

    // make a constant for wrath to stop at a certain distance
    const float EPSILON = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* LookAt makes Wrath look at the player. 
         * May need to erase this part because it will probably rotate his whole body. */
        transform.LookAt(mTarget.position, Vector3.up);
        //zRot = Mathf.Clamp()


     //    Vector3 difference = mTarget.position - transform.position;
    //    float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
     //   float rotationX = Mathf.Atan2(difference.z, difference.y) * Mathf.Rad2Deg;
     //  transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);

        /* this part should make wrath follow position. */
        if ((transform.position - mTarget.position).magnitude > EPSILON)
            transform.Translate(0.0f, 0.0f, mSpeed * Time.deltaTime);
    }
}
