using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;

public class PlayerMovement2 : MonoBehaviour
{
    Vector3 velocityThisFrame, velocity, acceleration;
    private float particleTimer, spritePosX, spritePosY;
    Vector3 spriteSize, spriteScaler;
    private bool inputFlip, collidingApplied;
    public GameObject particle, playerSprite;
    public List<Transform> wallList = new List<Transform>();
    private SpriteRenderer pcSpriteRenderer;
    [SerializeField] float movementIncrement, accelerationSpeed, maxSpeed, friction, camFlipTime,
        counterAccelerationModifier;
    [SerializeField] Vector2 particleTimerRange;
    
    
    // Start is called before the first frame update
    void Start()
    {
        pcSpriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        spriteSize.x = pcSpriteRenderer.bounds.size.x;
        spriteSize.y = pcSpriteRenderer.bounds.size.y;
        particleTimer = Random.Range(particleTimerRange.x, particleTimerRange.y);
        inputFlip = false;
        collidingApplied = false;
        spriteScaler = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        float angleVelocityVsAcceleration = Vector3.Angle(velocity, acceleration);
        float counterPushRatio = angleVelocityVsAcceleration / 180;
        acceleration = Vector3.zero; //zero out the acceleration at the beginning of each frame
        if(!inputFlip){
            if(Input.GetKey(KeyCode.W)) acceleration.y += 1 ; //W moves up
            else if(Input.GetKey(KeyCode.S)) acceleration.y -= 1 ; //S moves down
            else if(Input.GetKey(KeyCode.D)) acceleration.x += 1 ; //D moves right
            else if(Input.GetKey(KeyCode.A)) acceleration.x -= 1 ; //A moves left
        }else{ //input flipped
            if(Input.GetKey(KeyCode.W)) acceleration.y -= 1 ; //W moves down
            else if(Input.GetKey(KeyCode.S)) acceleration.y += 1 ; //S moves up
            else if(Input.GetKey(KeyCode.D)) acceleration.x -= 1 ; //D moves left
            else if(Input.GetKey(KeyCode.A)) acceleration.x += 1 ; //A moves right
        }

        acceleration = acceleration.normalized * accelerationSpeed * Time.deltaTime;
        velocity += acceleration * (1 + counterPushRatio * counterAccelerationModifier);
        if(acceleration == Vector3.zero){
            if(velocity.magnitude > friction * Time.deltaTime){
                velocity -= velocity.normalized * friction * Time.deltaTime;
            }else{
                velocity = Vector3.zero;
            }
        }

        Vector3 velocityThisFrame = velocity;
        Vector3 microVelocity;
        Vector3 positionNextFrame = transform.position;
        bool velXPositive = false;
        if(velocityThisFrame.x > 0) velXPositive = true;
        bool velYPositive = false;
        if(velocityThisFrame.y > 0) velYPositive = true;

        while(velocityThisFrame != Vector3.zero){ //when moving
            if(Mathf.Abs(velocityThisFrame.x)> Mathf.Abs(velocityThisFrame.y)){
                velocityThisFrame.x -= MovementIncrementSigned(velXPositive);
                microVelocity = new Vector3(MovementIncrementSigned(velXPositive), 0, 0);
                if(isThisPositionInWall(positionNextFrame + microVelocity)){
                    velocityThisFrame.x = 0;
                    if(!collidingApplied){
                        applyEffect();
                        collidingApplied = true;
                        Debug.Log("applied effect");
                    }
                }else{ 
                    positionNextFrame += microVelocity;
                    collidingApplied = false;
                }

                if(velocityThisFrame.x < movementIncrement && velocityThisFrame.x > -movementIncrement){
                    velocityThisFrame.x = 0;
                }
            }else{//if velocity y > velocity x
                velocityThisFrame.y -= MovementIncrementSigned(velYPositive);
                microVelocity = new Vector3(0, MovementIncrementSigned(velYPositive), 0);
                if(isThisPositionInWall(positionNextFrame + microVelocity)){
                    velocityThisFrame.y = 0;
                    if(!collidingApplied){
                        applyEffect();
                        collidingApplied = true;
                        Debug.Log("applied effect");
                    }
                }else{ 
                    positionNextFrame += microVelocity;
                    collidingApplied = false;
                }

                if(velocityThisFrame.y < movementIncrement && velocityThisFrame.y > -movementIncrement){
                    velocityThisFrame.y = 0;
                }
            }
            //particle
            if(particleTimer <= 0){
                particleTimer = Random.Range(particleTimerRange.x, particleTimerRange.y);
                Instantiate(particle, positionNextFrame, Quaternion.identity);
            }
        }//when moving

        //sprites
        spritePosX = 0; spritePosY = 0;
        spriteScaler = new Vector3(1, 1, 1);
        float scaler = 1 - 0.5f * velocity.magnitude / maxSpeed;
        if(Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x)){
            spriteScaler.y = scaler;
            if(velocity.y > 0) spritePosY += spriteSize.y/2 * (1 - scaler);
            else spritePosY -= spriteSize.y/2 * (1 - scaler);
        }else{
            spriteScaler.x = scaler;
            if(velocity.x > 0) spritePosX += spriteSize.x/2 * (1 - scaler);
            else spritePosX -= spriteSize.x/2 * (1 - scaler);
        }
        playerSprite.transform.localScale = spriteScaler;
        playerSprite.transform.localPosition = new Vector3(spritePosX, spritePosY, 0);

        if(velocity.magnitude > maxSpeed) velocity = velocity.normalized * maxSpeed;
        transform.position = positionNextFrame;
        particleTimer -= Time.deltaTime;
    }


    float MovementIncrementSigned(bool positiveDirection){
        if(positiveDirection) return movementIncrement;
        else return -movementIncrement;
    }

    bool isThisPositionInWall(Vector3 positionToCheck){ //true if it is a wall
        //New Method
        foreach(Transform currentWall in wallList){
            float xDist = Mathf.Abs(positionToCheck.x - currentWall.position.x);
            float yDist = Mathf.Abs(positionToCheck.y - currentWall.position.y);
            float xMaxDist = transform.localScale.x /2 + currentWall.localScale.x/2;
            float yMaxDist = transform.localScale.y /2 + currentWall.localScale.y/2;
            
            if(xDist < xMaxDist && yDist < yMaxDist){
                return true;
            }
        }
        return false;
    }

    void applyEffect(){
        GameObject closestWall = findClosetWall();
        if(closestWall.tag == "WallCamera"){
            //flipCamera();
        }else if(closestWall.tag == "WallBounce"){

        }else if(closestWall.tag == "WallFlip"){
            inputFlip = !inputFlip;
        }
    }

    GameObject findClosetWall(){
        float closestDistance = Mathf.Infinity;
        GameObject closestWall = null;
        foreach(Transform currentWall in wallList){
            float distance = Vector3.Distance(transform.position, currentWall.position);
            if(distance < closestDistance){
                closestDistance = distance;
                closestWall = currentWall.gameObject;
            }
        }
        return closestWall;
    }


    public IEnumerator flipCamera(){
        float step = 180;
        float totalDegree = 180;
        Vector3 camRot;
        float degreePerStep = totalDegree / step;
        for(int i = 0; i < step; i++){
            camRot = new Vector3(0, 0, degreePerStep);
            transform.Rotate(camRot);
            yield return new WaitForSeconds(camFlipTime/step);
        }
        StopCoroutine(flipCamera());
    }
}
