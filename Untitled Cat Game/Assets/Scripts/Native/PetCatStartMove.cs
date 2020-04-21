using System.Collections;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine;




/* this spript needs to: 
 * Make the cat sit UNTIL pet (collision of player and head) 
 * When cat is pet, update a trigger to "is walking" which will
 * make the cat get up and move, and also
 * it needs to call the Cinemachine cart script to make the cat move. 
 * */
public class PetCatStartMove : MonoBehaviour
{

    // Start is called before the first frame update
    /// <summary>The path to follow</summary>
    [Tooltip("The path to follow")]
    public CinemachinePathBase m_Path;
    bool moving;
    [SerializeField] private Animator myAnimator;
    /// <summary>This enum defines the options available for the update method.</summary>
    public enum UpdateMethod
    {
        /// <summary>Updated in normal MonoBehaviour Update.</summary>
        Update,
        /// <summary>Updated in sync with the Physics module, in FixedUpdate</summary>
        FixedUpdate,
        /// <summary>Updated in normal MonoBehaviour LateUpdate</summary>
        LateUpdate
    };

    /// <summary>When to move the cart, if Velocity is non-zero</summary>
    [Tooltip("When to move the cart, if Velocity is non-zero")]
    public UpdateMethod m_UpdateMethod = UpdateMethod.Update;

    /// <summary>How to interpret the Path Position</summary>
    [Tooltip("How to interpret the Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.")]
    public CinemachinePathBase.PositionUnits m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;

    /// <summary>Move the cart with this speed</summary>
    [Tooltip("Move the cart with this speed along the path.  The value is interpreted according to the Position Units setting.")]
    //[FormerlySerializedAs("m_Velocity")]
    public float m_Speed;

    /// <summary>The cart's current position on the path, in distance units</summary>
    [Tooltip("The position along the path at which the cart will be placed.  This can be animated directly or, if the velocity is non-zero, will be updated automatically.  The value is interpreted according to the Position Units setting.")]
    //[FormerlySerializedAs("m_CurrentDistance")]
    public float m_Position;

    void FixedUpdate()
    {
        if (m_UpdateMethod == UpdateMethod.FixedUpdate)
            SetCartPosition(m_Position + m_Speed * Time.deltaTime);
    }

    void Update()
    {
        float speed = Application.isPlaying ? m_Speed : 0;
        if (m_UpdateMethod == UpdateMethod.Update)
            SetCartPosition(m_Position + speed * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (!Application.isPlaying)
            SetCartPosition(m_Position);
        else if (m_UpdateMethod == UpdateMethod.LateUpdate)
            SetCartPosition(m_Position + m_Speed * Time.deltaTime);
    }

    void SetCartPosition(float distanceAlongPath)
    {
        if (m_Path != null && moving == true)
        {
            m_Position = m_Path.StandardizeUnit(distanceAlongPath, m_PositionUnits);
            transform.position = m_Path.EvaluatePositionAtUnit(m_Position, m_PositionUnits);
            transform.rotation = m_Path.EvaluateOrientationAtUnit(m_Position, m_PositionUnits);
        }
    }

void Start()
    {
Debug.Log("Start Cat move script");
        //myScript.enable = false;
        moving = false;
        myAnimator.SetBool("isWalking", false);
        myAnimator.SetBool("isSitting", true);
    }

    // Beginning of "I can't with this shit"

    // from https://answers.unity.com/questions/1305890/enabling-a-script-by-entering-a-trigger.html
    void OnTriggerEnter(Collider other)
    {
    Debug.Log("Hit it");
        if (other.gameObject.tag == "Player")
        {
		Debug.Log("Hit the cat");
            myAnimator.SetBool("isWalking", true);
            myAnimator.SetBool("isSitting", false);
            moving = true;
           
            // here we make the cat move. 
        }
    }

   /* void OnTriggerExit(Collision other)
    {
        
    }
    */
}
