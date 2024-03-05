using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    Vector3 velocityThisFrame, velocity, acceleration;
    public float counterAccelerationModifier;
    public List<Transform> wallList = new List<Transform>();
    private SpriteRenderer mySpriteRenderer;
    [SerializeField] float movementIncrement, accelerationSpeed, maxSpeed;
    public float friction = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float angleVelocityVsAcceleration = Vector3.Angle(velocity, acceleration);
        float counterPushRatio = angleVelocityVsAcceleration / 180;
        acceleration = Vector3.zero; //zero out the acceleration at the beginning of each frame

        if(Input.GetKey(KeyCode.W)) acceleration.y += 1 ; //W moves up
        else if(Input.GetKey(KeyCode.S)) acceleration.y -= 1 ; //S moves down
        else if(Input.GetKey(KeyCode.D)) acceleration.x += 1 ; //D moves right
        else if(Input.GetKey(KeyCode.A)) acceleration.x -= 1 ; //A moves left

        acceleration = acceleration.normalized * accelerationSpeed * Time.deltaTime;
        velocity += acceleration * (1 + counterPushRatio * counterAccelerationModifier);
        if(velocity.magnitude > maxSpeed) velocity = velocity.normalized * maxSpeed;
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

        while(velocityThisFrame != Vector3.zero){
            if(Mathf.Abs(velocityThisFrame.x)> Mathf.Abs(velocityThisFrame.y)){
                velocityThisFrame.x -= MovementIncrementSigned(velXPositive);
                microVelocity = new Vector3(MovementIncrementSigned(velXPositive), 0, 0);
                if(isThisPositionInWall(positionNextFrame + microVelocity)){
                    velocityThisFrame.x = 0;
                }else{ 
                    positionNextFrame += microVelocity;
                }

                if(velocityThisFrame.x < movementIncrement && velocityThisFrame.x > -movementIncrement){
                    velocityThisFrame.x = 0;
                }
            }else{//if velocity y > velocity x
                velocityThisFrame.y -= MovementIncrementSigned(velYPositive);
                microVelocity = new Vector3(0, MovementIncrementSigned(velYPositive), 0);
                if(isThisPositionInWall(positionNextFrame + microVelocity)){
                    velocityThisFrame.y = 0;
                }else{ 
                    positionNextFrame += microVelocity;
                }

                if(velocityThisFrame.y < movementIncrement && velocityThisFrame.y > -movementIncrement){
                    velocityThisFrame.y = 0;
                }
            }

            
        }

        transform.position = positionNextFrame;
    }


    float MovementIncrementSigned(bool positiveDirection){
        if(positiveDirection) return movementIncrement;
        else return -movementIncrement;
    }

    bool isThisPositionInWall(Vector3 positionToCheck){
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
}
