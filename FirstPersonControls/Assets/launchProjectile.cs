using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class launchProjectile : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    PlayerInput playerInput;
    bool projectileTriggered;
    Transform cameraFocus;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        cameraFocus = transform.Find("CameraFocus");
    }

    // Update is called once per frame
    void Update()
    {
        projectileTriggered = playerInput.actions["Projectile"].triggered;
        if(projectileTriggered){
            Instantiate(projectilePrefab, cameraFocus.transform.position +
            cameraFocus.transform.forward * 2, cameraFocus.transform.rotation);
        }
    }
}
