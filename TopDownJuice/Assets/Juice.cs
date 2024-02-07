using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juice : MonoBehaviour
{
    float squishTimer;
    float scaleYOriginal;
    [SerializeField] float squishMax;
    float scaleOld;
    [SerializeField] AnimationCurve animCurve;
    AudioSource myAudioSource;
    AudioClip baseClip;
    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        scaleOld = transform.localScale.y;   
        myAudioSource = GetComponent<AudioSource>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(squishTimer > 0){
            // float scaleY; 
            // if(squishTimer > .5f * squishMax){
            //     scaleY = scaleYOriginal * (squishTimer/squishMax);
            // }else{ //if inbetween .5f and 0, unsquish
            //     scaleY = scaleYOriginal * (1- squishTimer/squishMax);
            // }
            // transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
            // squishTimer -= Time.deltaTime;
            float scaleY = scaleYOriginal * animCurve.Evaluate(squishTimer/squishMax);
            transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
            squishTimer -= Time.deltaTime;
        }else{
            transform.localScale = new Vector3(transform.localScale.x, scaleOld, transform.localScale.z);
        }
        
    }

    void OnMouseDown(){
        Squish();
        myAudioSource.PlayOneShot(baseClip);
        mainCamera.GetComponent<CameraJuice>().BeginShake(0.1f, 0.1f, 0.1f, 0.1f);
        GetComponent<ParticleJuiceCreator>().CreateParticles(30);
    }

    void Squish(){
        if(squishTimer <= 0){
            squishTimer = 1f;
            scaleYOriginal = transform.localScale.y;
        }
    }
}
