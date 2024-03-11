using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] float speedMax, friction, lifeMax;
    public Vector3 velocity;
    private float timer;
    public SpriteRenderer spriteRenderer;
    public string particleType; 

    // Start is called before the first frame update
    void Start()
    {
        
        if(particleType == "Move"){
            spriteRenderer.color = new Color(Random.value, Random.value, Random.value, 1);
        }else if (particleType == "Break"){
            spriteRenderer.color = Color.HSVToRGB(0.3f, Random.Range(0.2f, 0.4f), Random.Range(0.5f, 0.9f));
            speedMax *= Random.Range(7f, 9f);
            lifeMax *= Random.Range(0.1f, 0.5f);
            float randomSize = Random.Range(0.4f, 0.8f);
            transform.localScale = new Vector3(randomSize, randomSize, 1);
        }else if (particleType == "Bounce"){
            spriteRenderer.color = Color.HSVToRGB(0.6f, Random.Range(0.2f, 0.4f), Random.Range(0.5f, 0.9f));
            speedMax *= Random.Range(7f, 9f);
            lifeMax *= Random.Range(0.1f, 0.5f);
            float randomSize = Random.Range(0.1f, 0.5f);
            transform.localScale = new Vector3(randomSize, randomSize, 1);
        }else if(particleType == "Trincket"){
            spriteRenderer.color = Color.HSVToRGB(Random.Range(0.0f, 0.15f), Random.Range(0.75f, 1.0f), Random.Range(0.8f, 1.0f));
            speedMax *= Random.Range(0.3f, 0.8f);
            lifeMax *= Random.Range(1f, 8f);
            float randomSize = Random.Range(0.1f, 0.2f);
            transform.localScale = new Vector3(randomSize, randomSize, 1);
        }else if(particleType == "TrincketBurst"){
            spriteRenderer.color = Color.HSVToRGB(Random.Range(0.0f, 0.15f), Random.Range(0.75f, 1.0f), Random.Range(0.8f, 1.0f));
            speedMax *= Random.Range(7f, 9f);
            lifeMax *= Random.Range(0.1f, 0.5f);
            float randomSize = Random.Range(0.4f, 0.8f);
            transform.localScale = new Vector3(randomSize, randomSize, 1);
        }
        velocity = initializeVelocity();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        float velocityX = Random.Range(-1f, 1f);
        float velocityY = Random.Range(-1f, 1f);
        Vector3 v = new Vector3(velocityX, velocityY, 0);
        if(v.magnitude > 1.0f) v = v.normalized;
        v = v * speedMax;
        return v;
    }

}
