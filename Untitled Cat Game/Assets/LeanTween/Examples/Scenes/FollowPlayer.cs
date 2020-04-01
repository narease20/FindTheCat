using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform mTarget;

    public float mSpeed = 5.0f;

    // make a constant for wrath to stop at a certain distance - Currently makeing it a float so it can be changed in the editor
    [SerializeField, Range(1,10)]
    public float EPSILON = 1.0f;

    public bool follow = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (follow)
        {
            // Hey future Isaiah, should you make it so follow gets set off when the player is not in raycast range
            // so Wrath should speen around to try and see the player?
            FollowDesignated();
        }

    }

    void FollowDesignated()
    {
        /* LookAt makes Wrath look at the player. 
        * May need to erase this part because it will probably rotate his whole body. */
        transform.LookAt(new Vector3(mTarget.position.x, 0, mTarget.position.z), Vector3.up);
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
