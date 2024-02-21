using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float cameraSpeed, switchDirectionThreshold, forwardLead;
    bool goingRight = true, goingLeft = true;
    Vector3 targetPosition;
    float farthestReach;

    private void Start(){
        targetPosition = transform.position;
    }

    private void FixedUpdate(){
        if(goingRight){
            if(player.position.x > farthestReach){
                farthestReach = player.position.x;
                targetPosition.x = farthestReach + forwardLead;
            }
            if(player.position.x < farthestReach - switchDirectionThreshold){
                goingRight = false;
            }
        }else{
            if(player.position.x < farthestReach){
                farthestReach = player.position.x;
                targetPosition.x = farthestReach - forwardLead;
            }
            if(player.position.x > farthestReach + switchDirectionThreshold){
                goingRight = true;
            }
        }
       



        //targetPosition.x = player.position.x;
        targetPosition.y = player.position.y;
        float percentageToMove = cameraSpeed * Time.deltaTime;
        //the same as the lerp function
        transform.position = targetPosition * percentageToMove + transform.position * (1 - percentageToMove);
    }
}
