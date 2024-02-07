using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceIn : MonoBehaviour
{
    Vector3 goalPosition, offScreenPosition;
    [SerializeField]float holdTimer = 3f;
    [SerializeField]float bounceTimer = 1f;
    [SerializeField]AnimationCurve animCurve;
    float bounceTimerMax = 3f;

    // Start is called before the first frame update
    void Start()
    {
        goalPosition = transform.position;
        float offScreenPositionY = goalPosition.y + (Camera.main.orthographicSize * 2);
        offScreenPosition = new Vector3(goalPosition.x, offScreenPositionY, goalPosition.z);
        transform.position = offScreenPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(holdTimer > 0){
            holdTimer -= Time.deltaTime;
        }else{
            if(bounceTimer < bounceTimerMax){
                float timeRatio = bounceTimer / bounceTimerMax;
                float animOutput = animCurve.Evaluate(timeRatio);
                transform.position = goalPosition * animOutput + offScreenPosition * (1- animOutput);

                bounceTimer += Time.deltaTime;
            }else{
                transform.position = goalPosition;
            }
        }
    }
}
