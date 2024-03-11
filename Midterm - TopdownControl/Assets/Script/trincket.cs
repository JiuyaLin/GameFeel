using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trincket : MonoBehaviour
{
    [SerializeField] float particleTimer;
    public GameObject particle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(particleTimer < 0){
            GameObject newParticle = Instantiate(particle, transform.position, Quaternion.identity);
            newParticle.GetComponent<Particle>().particleType = "Trincket";
            particleTimer = Random.Range(0.1f, 0.5f);
        }
        particleTimer -= Time.deltaTime;
    }
}
