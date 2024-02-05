using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public List<Transform> wallList = new List<Transform>();


    Vector3 acceleration, velocity;
    public float accelerationSpeed;
    public float maxSpeed;
    public float friction;
    public float counerAccelerationMod;
    

    // Start is called before the first frame update
    void Start()
    {
        //accelerationSpeed = accelerationSpeed / Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("accleration" + acceleration);
        //Debug.Log("velocity" + velocity);

        //input
        acceleration = Vector3.zero; //zero out the acceleration
        if(Input.GetKey(KeyCode.UpArrow)) acceleration.y += 1;
        if(Input.GetKey(KeyCode.DownArrow)) acceleration.y -= 1;
        if(Input.GetKey(KeyCode.RightArrow)) acceleration.x += 1;
        if(Input.GetKey(KeyCode.LeftArrow)) acceleration.x -= 1;

        //update velocity
        float counterAcceleration = Vector3.Angle(velocity, acceleration) / 180 * counerAccelerationMod; //larger the angle, the more counter acceleration
        acceleration = acceleration.normalized * accelerationSpeed; //normalize acceleration
        velocity += acceleration * (1 - counterAcceleration); //add acceleration to velocity
        if(velocity.magnitude > maxSpeed) velocity = velocity.normalized * maxSpeed; //cap velocity
        if(acceleration == Vector3.zero){ //slowdown
            if(velocity.magnitude > friction){
                velocity -= velocity.normalized * friction;
            }
            else velocity = Vector3.zero;
        }

        transform.position += velocity * Time.deltaTime; //move the player
    
    }
}
