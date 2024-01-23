using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 velocity;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.zero; //zeo out the velocity at the beginning of each frame

        if(Input.GetKey(KeyCode.W)) velocity.y += 1 ; //W moves up
        if(Input.GetKey(KeyCode.S)) velocity.y -= 1 ; //S moves down
        if(Input.GetKey(KeyCode.D)) velocity.x += 1 ; //D moves right
        if(Input.GetKey(KeyCode.A)) velocity.x -= 1 ; //A moves left
        
        Vector3 nextPosition = transform.position + velocity;

        transform.position = nextPosition;
    }
}
