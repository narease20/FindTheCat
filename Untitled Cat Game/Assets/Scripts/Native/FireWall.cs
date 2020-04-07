using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FireWall : MonoBehaviour
{
    public bool canBeBurned = true;
    Rigidbody rb;

    public GameObject fireBox;
    public float fireBoxYPos;
    [SerializeField, Range(-5f, 0f)]
    public float fireBoxOffset;
    bool moveFireBox = false;

    public Material myMaterial;
    public ParticleSystem spawnFire;
    public ParticleSystem deadBush;

    [SerializeField, Range(0.1f, 10f)]
    public float burnTime = 5f;

    private void Awake()
    {
        myMaterial.color = Color.red;
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.angularDrag = 0;

        fireBoxYPos = fireBox.transform.position.y;

    }

    private void Start()
    {
        fireBox.transform.position = new Vector3(fireBox.transform.position.x, fireBox.transform.position.y + fireBoxOffset, fireBox.transform.position.z);
        Mathf.Clamp(fireBox.transform.position.y, fireBox.transform.position.y - burnTime, fireBoxYPos);
    }

    private void Update()
    {
        if (moveFireBox && fireBox.transform.position.y < fireBoxYPos)
        {
            fireBox.transform.position = new Vector3(fireBox.transform.position.x, fireBox.transform.position.y + (Time.deltaTime*3), fireBox.transform.position.z);
        }
    }

    public void DestroyMe()
    {
        moveFireBox = true;
        Instantiate(spawnFire, transform.position, transform.rotation, transform);
        myMaterial.color = Color.Lerp(Color.red, Color.black, burnTime);
        // Something something make the wall change color, instanciate a flame, then die out.
        StartCoroutine(FinalBurn());
    }

    IEnumerator FinalBurn()
    {
        yield return new WaitForSeconds(burnTime);
        gameObject.SetActive(false);
        //SceneRecords SR = GetComponent<SceneRecords>();
        //SR.ParticleMaker(deadBush, gameObject.transform, burnTime);
        myMaterial.color = Color.red;
        //Destroy(spawnFire, 1);
        //Destory(this);
    }

}
