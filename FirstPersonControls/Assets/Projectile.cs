using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed; //unit m/s
    Rigidbody rb;
    public Vector3 projectileSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        projectileSpeed = transform.forward * Time.fixedDeltaTime * speed;
        rb.MovePosition(transform.position + projectileSpeed);
    }

    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.tag == "Blocker"){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Enemytarget"){
            Destroy(other.gameObject);
            InstantiateFruit.MelleKill++;
            InstantiateFruit.totalFruitNum--;
        }
    }
}
