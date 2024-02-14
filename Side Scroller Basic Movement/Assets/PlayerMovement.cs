using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput myPlayerInput;
     // Hrozontal input, no vertical input
    float inputX;
    Rigidbody2D rb;
    [SerializeField] float speed, gravity, jumpStartingPush;
    Vector2 velocity;
    bool jumpHeld, jumpTriggered, onPlatform, jumping;

    // Start is called before the first frame update
    void Start()
    {
        myPlayerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() //anything input related
    {
        //happen everytime re-draw
        inputX = myPlayerInput.actions["Move"].ReadValue<Vector2>().x;
        jumpHeld = myPlayerInput.actions["Jump"].ReadValue<float>() > 0.9f;
        jumpTriggered = myPlayerInput.actions["Jump"].triggered;
        if(jumpTriggered){
            velocity.y = jumpStartingPush;
        }
    }

    void FixedUpdate() //anything physics related
    {
        //happens way more often than fixed update
        //rb moves by sliding not teleporting
        velocity.x = inputX * speed * Time.fixedDeltaTime; 
        velocity.y -= gravity * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + velocity);
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Platform"){
            foreach(ContactPoint2D contact in collision.contacts){
                Debug.DrawRay(contact.point, contact.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 1f);
                //checking normal if the player hits vertically
                if(Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x)){
                    if(contact.normal.y > 0){ //hit above?
                        onPlatform = true; //landed on top fo platform
                        if(velocity.y < 0) velocity.y = 0; //stop going down
                    }else //hit below?
                    {
                        //TODO: check right vs. left
                        //wall jump?
                        if(velocity.y > 0) velocity.y = 0; //stop going up
                    }
                }
            }
        }
    }
}
