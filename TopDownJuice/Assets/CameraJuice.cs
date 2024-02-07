using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraJuice : MonoBehaviour
{
    [SerializeField] float shakeTimer, offsetXMax, offsetYMax, rotationMax;
    bool traumaused;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(shakeTimer > 0){
            float trauma = 1;
            //if(traumaused) trauma = (1+shakeTimer) * (1+shakeTimer);

            float shakeX = trauma * (Mathf.PerlinNoise1D(Time.time) * offsetXMax - (offsetXMax/2));
            float shakeY = trauma * (Mathf.PerlinNoise1D(Time.time + 1000) * offsetYMax - (offsetYMax/2));

            //float shakeX = Random.Range(-offsetXMax, offsetXMax);
            //float shakeY = Random.Range(-offsetYMax, offsetYMax);
            transform.localPosition = new Vector3(shakeX, shakeY, transform.localPosition.z);

            float angle = Random.Range(-rotationMax, rotationMax);
            transform.localRotation = Quaternion.Euler(0,0,angle);

            shakeTimer -= Time.deltaTime;

            if(shakeTimer <= 0){
                shakeTimer = 0;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
    }

    public void BeginShake(float shakeTimer, float offsetXMax, float offsetYMax, float rotationMax){
        this.shakeTimer = shakeTimer;
        this.offsetXMax = offsetXMax;
        this.offsetYMax = offsetYMax;
        this.rotationMax = rotationMax; 
    }
}
