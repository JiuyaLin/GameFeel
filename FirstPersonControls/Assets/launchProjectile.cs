using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class launchProjectile : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    PlayerInput playerInput;
    bool projectileTriggered;
    Transform cameraHolder;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        cameraHolder = transform.Find("Camera Holder");
    }

    // Update is called once per frame
    void Update()
    {
        projectileTriggered = playerInput.actions["Projectile"].triggered;
        if(projectileTriggered){
            Instantiate(projectilePrefab, cameraHolder.transform.position +
            cameraHolder.transform.forward * 5, cameraHolder.transform.rotation);
        }
    }
}
