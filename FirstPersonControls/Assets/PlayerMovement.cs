using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;
    PlayerInput playerInput;
    float axisHorizontal, axisForward;
    Vector3 velocity, velocityInput, velocityGravity, velocitySpecial;
    [SerializeField] float speed;
    [SerializeField] float jumpVelocity, gravity, jumpHeight;
    bool jumpTriggered, knifeRiding;
    Vector3 ridingOffset;
    GameObject ridingProjectile;

    // Start is called before the first frame update
    void Start()
    {
        knifeRiding = false;
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        jumpVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
    }

    // Update is called once per frame
    void Update()
    {
        axisHorizontal = playerInput.actions["Move"].ReadValue<Vector2>().x;
        axisForward = playerInput.actions["Move"].ReadValue<Vector2>().y;
        jumpTriggered = playerInput.actions["Jump"].triggered;

        if(jumpTriggered && characterController.isGrounded){
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.E) && knifeRiding){ //Get Off Knife
            knifeRiding = false;
        }
    }

    void FixedUpdate(){
        // velocity = new Vector3(axisHorizontal, 0, axisForward);
        // characterController.Move(velocity * Time.deltaTime);
        Vector3 forwardInput = transform.forward * axisForward;
        Vector3 horizontalInput = transform.right * axisHorizontal;
        velocityInput = forwardInput + horizontalInput;
        velocityInput.Normalize();
        velocityInput *= speed;

        velocityGravity.y -= gravity * Time.fixedDeltaTime;
        if(characterController.isGrounded && velocityGravity.y < 0){
            velocityGravity.y = -2f;
        }
        velocityInput = velocityInput + velocityGravity + velocitySpecial;

        if(!knifeRiding){ //move the player normally
            characterController.Move(velocityInput * Time.deltaTime);
        }else if (knifeRiding && ridingProjectile != null){ //riding a knife
            knifeRideMoving();
        }else{ //knife destroyed by wall
            knifeRiding = false;
        }
    }

    void Jump(){
        velocityGravity.y = jumpVelocity;
    }

    void knifeRideMoving(){
        characterController.Move(ridingProjectile.GetComponent<Projectile>().projectileSpeed);
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Projectile"){
            ridingProjectile = other.gameObject;
            knifeRiding = true;
        }
    }

    // void OnCollisionEnter(Collision collision){
    //     if (collision.gameObject.tag == "Projectile"){
    //         ridingProjectile = collision.gameObject;
    //     }
    // }


}
