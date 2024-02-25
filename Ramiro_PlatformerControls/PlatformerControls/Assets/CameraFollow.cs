using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float cameraSpeed, switchDirectionThreshold, forwardLead;
    bool goingRight = true;
    Vector3 targetPosition;
    float farthestReach, shakeTimer;

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
                //StartCoroutine(shakeCorutine(0.1f, 0.1f, 0.1f, 1f));
            }
        }

        targetPosition.y = player.position.y; //the camera just tries to follow the player's Y position - nothing special about this one
        float percentageToMove = cameraSpeed * Time.fixedDeltaTime; //camera speed sets what percentage of the way the camera will move between its current position and the target position
        transform.position = targetPosition * percentageToMove + transform.position * (1 - percentageToMove); //lerp the camera between its current position and the target position
    }

    //a coroutine that shakes the camera for a short time
    public IEnumerator shakeCorutine(float shakeTimerMax, float offsetXMax, float offsetYMax, float rotationMax){
        Debug.Log("coroutine started");
        float shakeTimer = shakeTimerMax;
        Vector3 preShakeLoc = transform.localPosition;
        while(shakeTimer > 0){
            Debug.Log("shaking");
            shakeTimer -= Time.deltaTime;
            float impact = 1;
            float shakeX = impact * (Mathf.PerlinNoise1D(Time.time) * offsetXMax - (offsetXMax/2));
            float shakeY = impact * (Mathf.PerlinNoise1D(Time.time + 1000) * offsetYMax - (offsetYMax/2));
            transform.localPosition += new Vector3(shakeX, shakeY, 0);
            float angle = Random.Range(-rotationMax, rotationMax);
            transform.localRotation = Quaternion.Euler(0,0,angle);
            yield return new WaitForSeconds(.01f);
        }
        Debug.Log("coroutine ended");
        transform.localPosition = preShakeLoc;
        transform.localRotation = Quaternion.identity;
        StopCoroutine(shakeCorutine(shakeTimerMax, offsetXMax, offsetYMax, rotationMax));
    }
}

