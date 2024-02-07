using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleJuice : MonoBehaviour
{
    float timerMax, timer;
    Vector3 velocity;
    Material material;


    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        timerMax = Random.Range(1f, 2f);
        timer = timerMax;
        velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(timer <= 0) Destroy(gameObject);

        transform.position += velocity * Time.deltaTime;
        material.color = new Color(material.color.r, material.color.g, material.color.b, timer/timerMax);

        timer -= Time.deltaTime;
    }
}
