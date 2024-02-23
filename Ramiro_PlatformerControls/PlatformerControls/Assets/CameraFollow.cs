using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float cameraSpeed, switchDirectionThreshold, forwardLead;
    bool goingRight = true;
    Vector3 targetPosition;
    float farthestReach;

    private void Start() {
        targetPosition = transform.position; //set default position for target
    }

    private void FixedUpdate() {
        if (goingRight) {
            if (player.position.x > farthestReach) { //if player has moved farther than it has ever been, move the target position forward
                farthestReach = player.position.x;
                targetPosition.x = player.position.x + forwardLead; //the lead makes the camera look ahead of the player
            }

            if(player.position.x < farthestReach - switchDirectionThreshold) { //if the player has move far enough back, switch direction
                goingRight = false;
            }
        }
        else { //going left
            if (player.position.x < farthestReach) { //if player has moved farther back than it has ever been, move the target position back
                farthestReach = player.position.x;
                targetPosition.x = player.position.x - forwardLead; //the lead makes the camera look behind the player
            }

            if(player.position.x > farthestReach + switchDirectionThreshold) { //if the player has move far enough forward, switch direction
                goingRight = true;
            }

        }
        


        targetPosition.y = player.position.y; //the camera just tries to follow the player's Y position - nothing special about this one
        float percentageToMove = cameraSpeed * Time.fixedDeltaTime; //camera speed sets what percentage of the way the camera will move between its current position and the target position
        transform.position = targetPosition * percentageToMove + transform.position * (1 - percentageToMove); //lerp the camera between its current position and the target position
    }
}
