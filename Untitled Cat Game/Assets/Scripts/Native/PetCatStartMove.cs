using System.Collections;
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
    // I think this is how I make it so I can call the script?
    //public class CinemachineDollyCart;
    //public UnityScript MyScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /* Beginning of "I can't with this shit"

    // from https://answers.unity.com/questions/1305890/enabling-a-script-by-entering-a-trigger.html
    void OnTriggerEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            MyScript myScript = other.gameObject.GetComponent<CinemachineDollyCart>();
            //You could check if you really attached MyScript here, but I think we skip that. ;-)
            myScript.enabled = true;
        }
    }

    void OnTriggerExit(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            MyScript myScript = other.gameObject.GetComponent<MyScript>();
            myScript.enabled = false;
        }
    }

    End of "I can't with this shit"
    */

    // Update is called once per frame
    void Update()
    {
        
    }
}
