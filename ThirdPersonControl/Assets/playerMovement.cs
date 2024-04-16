using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    Vector2 move;
    PlayerInput playerInput;
    CharacterController characterController;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Transform cameraTransform, lookDirection;
    Vector3 velocity, velocityInput, velocityGravity;
    bool cameraIsAction;
    [SerializeField] CinemachineFreeLook regularCamera, actionCamera;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        move = playerInput.actions["Move"].ReadValue<Vector2>();
        if(playerInput.actions["Fire"].triggered){
            SwitchCamera();
        }

        Vector3 viewDirection = transform.position 
        - new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
        lookDirection.forward = viewDirection;

        transform.forward = Vector3.Slerp(transform.forward, lookDirection.forward, 10 * Time.deltaTime);

        velocityInput = lookDirection.forward * move.y + lookDirection.right * move.x;
        velocityGravity *= moveSpeed;

        velocity = velocityInput + velocityGravity;

        characterController.Move(velocity * Time.deltaTime);
    }

    void SwitchCamera(){
        if(cameraIsAction){
            cameraIsAction = false;
            actionCamera.Priority = 0;
            regularCamera.Priority = 1;
        }
        else{
            cameraIsAction = true;
            actionCamera.Priority = 1;
            regularCamera.Priority = 0;
        }
    }
}
