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
    bool jumpTriggered;

    // Start is called before the first frame update
    void Start()
    {
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
        characterController.Move(velocityInput * Time.deltaTime);
    }

    void Jump(){
        velocityGravity.y = jumpVelocity;
    }


}
