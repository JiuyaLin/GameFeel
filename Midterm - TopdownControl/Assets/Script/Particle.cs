using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] float speedMax, friction, lifeMax;
    private Vector3 velocity;
    private float timer;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        velocity = initializeVelocity();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(Random.value, Random.value, Random.value, 1);
        timer = lifeMax;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        //velocity *= 1 - friction*Time.deltaTime;
        timer -= Time.deltaTime;
        if(timer <= 0) Destroy(gameObject);
        
    }

    Vector3 initializeVelocity(){
        float velocityX = Random.Range(-1, 1);
        float velocityY = Random.Range(-1, 1);
        Vector3 v = new Vector3(velocityX, velocityY, 0);
        v = v.normalized * speedMax;
        return v;
    }
}
