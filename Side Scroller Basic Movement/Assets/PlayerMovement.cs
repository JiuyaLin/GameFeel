using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput myPlayerInput;
     // Hrozontal input, no vertical input
    float inputX;
    Rigidbody2D rb;
    [SerializeField] float speed, gravity, jumpStartingPush, jumpHeldTimerMax, fallVelocityMax, 
    jumpPreloadTimerMax, coyoteTimerMax, apexTimerMax;
    Vector2 velocity;
    bool jumpHeld, jumpTriggered, onPlatform, jumping, apexReached, enterPlatformJumpNotResetYet;
    float jumpHeldTimer, jumpPreloadTimer, coyoteTimer, apexTimer;
    [SerializeField] BoxCollider2D footBoxCollier;


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
            if(!onPlatform){
                if(coyoteTimer > 0)beginJump();
                else jumpPreloadTimer = jumpPreloadTimerMax;
            }else{
                beginJump();
            }
        }

        jumpPreloadTimer -= Time.deltaTime;
        coyoteTimer -= Time.deltaTime;
    }

    void FixedUpdate() //anything physics related
    {
        //happens way more often than fixed update
        //rb moves by sliding not teleporting
        velocity.x = inputX * speed * Time.fixedDeltaTime; 
        if(jumping && jumpHeldTimer < jumpHeldTimerMax){ //no gravity
            if(jumpHeld) jumpHeldTimer += Time.fixedDeltaTime;
            else jumpHeldTimer = jumpHeldTimerMax;
        }else if(apexReached && apexTimer > 0){
            apexTimer -= Time.fixedDeltaTime;
            velocity.y -= .5f * gravity * Time.fixedDeltaTime;
            if(apexTimer <= 0) footBoxCollier.enabled = true;
        }else {
            velocity.y -= gravity * Time.fixedDeltaTime;
            if(velocity.y < -fallVelocityMax) velocity.y = -fallVelocityMax;
            if(!apexReached && jumping && velocity.y <= 0){
                apexReached = true;
                apexTimer = apexTimerMax;
            }
        }
        rb.MovePosition(rb.position + velocity);
    }

    private void OnCollisionStay2D(Collision2D collision){
        if(collision.gameObject.tag == "Platform"){
            foreach(ContactPoint2D contact in collision.contacts){
                Debug.DrawRay(contact.point, contact.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 1f);
                //checking normal if the player hits vertically
                if(Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x)){
                    if(contact.normal.y > 0){ //hit above?
                        onPlatform = true; //landed on top fo platform
                        if(velocity.y < 0) velocity.y = 0; //stop going down
                        if(jumpPreloadTimer > 0) beginJump();
                        if(enterPlatformJumpNotResetYet){
                            resetJump();
                            enterPlatformJumpNotResetYet = false;
                        }
                    }else //hit below?
                    {
                        //TODO: check right vs left. maybe use this for wall jummp?
                    }
                }
            }
        }
        
    }

    void beginJump(){
        velocity.y = jumpStartingPush;
        jumping = true;
        jumpHeldTimer = 0;
        footBoxCollier.enabled = false;
    }

    void resetJump(){
        jumping = false;
        apexReached = false;
    } 

    void OnCollisionEnter2D (Collision2D collision){
        if(collision.gameObject.tag == "Platform"){
            //onPlatform = true;
            //resetJump();
            enterPlatformJumpNotResetYet = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision){
        if(collision.gameObject.tag == "Platform"){
            if(onPlatform){
                onPlatform = false;
                coyoteTimer = coyoteTimerMax;
                enterPlatformJumpNotResetYet = false;
            }
        }
    }
}
