using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement2 : MonoBehaviour
{
    public List<Transform> wallList = new List<Transform>();
    public GameObject anchor;

    Vector3 acceleration, velocity = Vector3.zero;
    //acceleration = (r, theta)
    public float accelerationSpeed;
    public float maxSpeed;
    public float friction;
    public float counterAccelerationMod;
    public float thetaSpeedMod;
    private Vector3 lastRTheta = Vector3.zero; //store the r, theta last position
    public bool clockwiseRotation;
    public bool isPlayer; //capsul center is not a player

    float movementIncrement;
    

    // Start is called before the first frame update
    void Start()
    {
        movementIncrement = 0.001f * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("accleration" + acceleration);
        //Debug.Log("velocity" + velocity);
        Vector3 oldPosition = transform.position;

        //input
        acceleration = Vector3.zero; //zero out the acceleration
        if(isPlayer){
            if(Input.GetKey(KeyCode.W)) acceleration.x += 1; //acceleration in r
            if(Input.GetKey(KeyCode.S)) acceleration.x -= 1;
            float thetaRotation = 1f;
            if(!clockwiseRotation) thetaRotation = -thetaRotation;
            if(Input.GetKey(KeyCode.D)) acceleration.y += thetaRotation; //acceleration in theta
            if(Input.GetKey(KeyCode.A)) acceleration.y -= thetaRotation;
        }else{
            if(Input.GetKey(KeyCode.UpArrow)) acceleration.y += 1;
            if(Input.GetKey(KeyCode.DownArrow)) acceleration.y -= 1;
            if(Input.GetKey(KeyCode.RightArrow)) acceleration.x += 1;
            if(Input.GetKey(KeyCode.LeftArrow)) acceleration.x -= 1;
        }

        //update velocity
        float counterAcceleration = Vector3.Angle(velocity, acceleration) / 180 * counterAccelerationMod; //larger the angle, the more counter acceleration
        acceleration = acceleration.normalized * accelerationSpeed; //normalize acceleration
        velocity += acceleration * (1 - counterAcceleration); //add acceleration to velocity
        if(velocity.magnitude > maxSpeed) velocity = velocity.normalized * maxSpeed; //cap velocity
        if(acceleration == Vector3.zero){ //slowdown
            if(velocity.magnitude > friction){
                velocity -= velocity.normalized * friction;
            }
            else velocity = Vector3.zero;
        }
        
        Vector3 newPosition = Vector3.zero;
        //if player, update position into polar coordinates
        if(isPlayer){
            Vector2 newXY = Vector2.zero;
            Vector2 newRTheta = Vector2.zero;
            newRTheta.x = lastRTheta.x + velocity.x* Time.deltaTime;
            newRTheta.y = lastRTheta.y + velocity.y/180*(2*3.14f) * Time.deltaTime * (1+ thetaSpeedMod);
            newXY.x = newRTheta.x * Mathf.Cos(newRTheta.y);
            newXY.y = newRTheta.x * Mathf.Sin(newRTheta.y);
            newPosition.x = anchor.transform.position.x + newXY.x;
            newPosition.y = anchor.transform.position.y + newXY.y;
            lastRTheta = new Vector3(newRTheta.x, newRTheta.y, 0);
            transform.position = newPosition; //move the player
        }else{
           newPosition = transform.position + velocity * Time.deltaTime; 
        }


        //the below code will make the engione extremely inefficient and stuck
        /*
        //check if in wall
        Vector3 velocityThisFrame = velocity;
        Vector3 microVelocity;
        newPosition = transform.position;
        bool velXPositive = false; //check direction of velocity
        if(velocityThisFrame.x > 0) velXPositive = true;
        bool velYPositive = false;
        if(velocityThisFrame.y > 0) velYPositive = true;
        if(!isPlayer){
            while (velocityThisFrame != Vector3.zero){
                if(Mathf.Abs(velocityThisFrame.x)> Mathf.Abs(velocityThisFrame.y)){
                    velocityThisFrame.x -= MovementIncrementSigned(velXPositive);
                    microVelocity = new Vector3(MovementIncrementSigned(velXPositive), 0, 0);
                    if(isThisInWall(newPosition + microVelocity)) velocityThisFrame.x = 0;
                    else newPosition += microVelocity;
                    if(velocityThisFrame.x < movementIncrement && velocityThisFrame.x > -movementIncrement){
                        velocityThisFrame.x = 0; }
                }else{//if velocity y > velocity x
                    velocityThisFrame.y -= MovementIncrementSigned(velYPositive);
                    microVelocity = new Vector3(0, MovementIncrementSigned(velYPositive), 0);
                    if(isThisInWall(newPosition + microVelocity)) velocityThisFrame.y = 0;
                    else newPosition += microVelocity;
                    if(velocityThisFrame.y < movementIncrement && velocityThisFrame.y > -movementIncrement){
                        velocityThisFrame.y = 0;}
                }            
            }
        } */
        


        transform.position = newPosition; //move the player
    }
    float MovementIncrementSigned(bool positiveDirection){
        if(positiveDirection) return movementIncrement;
        else return -movementIncrement;
    }

    bool isThisInWall(Vector3 position){
        foreach(Transform wall in wallList){
            float xDist = Mathf.Abs(position.x - wall.position.x);
            float yDist = Mathf.Abs(position.y - wall.position.y);
            float xMax = transform.localScale.x/2 + wall.localScale.x/2;
            float yMax = transform.localScale.y/2 + wall.localScale.y/2;
            if(xDist < xMax && yDist < yMax){
                return true;
            }
        }
        return false;
    }
}
