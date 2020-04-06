using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireWall : MonoBehaviour
{
    public bool canBeBurned = true;
    Rigidbody rb;

    public Material myMaterial;
    public ParticleSystem spawnFire;

    [SerializeField, Range(0.1f, 5f)]
    public float burnTime = 2f;

    //X: 92.93 Y: 1.09 Z: 111.74

    private void Awake()
    {
        myMaterial.color = Color.red;
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.angularDrag = 0;
    }

    public void DestroyMe()
    {
        Instantiate(spawnFire, transform.position, transform.rotation, transform);
        myMaterial.color = Color.Lerp(Color.red, Color.black, burnTime);
        // Something something make the wall change color, instanciate a flame, then die out.
        StartCoroutine(FinalBurn());
    }

    IEnumerator FinalBurn()
    {
        yield return new WaitForSeconds(burnTime);
        gameObject.SetActive(false);
        myMaterial.color = Color.red;
        //Destroy(spawnFire, 1);
        //Destory(this);
    }

}
