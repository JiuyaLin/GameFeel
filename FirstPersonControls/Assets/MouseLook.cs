using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    PlayerInput playerInput;
    float inputHorizontal, inputVertical, xRotation;
    [SerializeField] float mouseSensitivity;
    [SerializeField] Transform cameraHolder;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        inputHorizontal = playerInput.actions["Look"].ReadValue<Vector2>().x;
        inputVertical = playerInput.actions["Look"].ReadValue<Vector2>().y;

        float inputHorizontalThisFrame = inputHorizontal * mouseSensitivity * Time.deltaTime;
        float inputVerticalThisFrame = inputVertical * mouseSensitivity * Time.deltaTime;

        //x Rotation: up Down
        xRotation -= inputVerticalThisFrame;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //y Rotation: left right
        transform.Rotate(Vector3.up * inputHorizontalThisFrame);
        
    }
}
