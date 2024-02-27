using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput myPlayerInput;
    float inputX, inputY;
    Rigidbody2D rb;
    [SerializeField] float speed, gravity, jumpStartingPush, jumpHeldTimerMax, fallSpeedMax, 
                        jumpPreloadTimerMax, coyoteTimerMax, apexTimerMax, wallTimerMax, lastJumpPressedTimerMax,
                        wallJumpScalerX, wallJumpScalerY;
    Vector2 velocity;
    bool jumpHeld, jumpTriggered, apexReached, enteredPlatformJumpNotResetYet;
    bool onPlatform, jumping, climbing, wallJumping;
    float jumpHeldTimer, jumpPreloadTimer, coyoteTimer, apexTimer, airTimer, lastJumpPressedTimer;
    [SerializeField] BoxCollider2D footBoxCollider;
    public GameObject camera;
    CameraFollow cameraFollow;
    public GameObject playerVisual;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        //get access to the PlayerInput and Rigidbody2D components
        myPlayerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        lastJumpPressedTimer = lastJumpPressedTimerMax; //if a jump is pressed within this time, it will be > 0
        cameraFollow = camera.GetComponent<CameraFollow>();
        spriteRenderer = playerVisual.GetComponent<SpriteRenderer>();
    }

    //all our input logic and one-time actions that are based on inputs are in update
    private void Update()
    {
        //get values from inputs
        inputX = myPlayerInput.actions["Move"].ReadValue<Vector2>().x; //analog stick/WASD horizontal input for left/right movement
        inputY = myPlayerInput.actions["Move"].ReadValue<Vector2>().y; //analog stick/WASD vertical input for up/down movement
        jumpHeld = myPlayerInput.actions["Jump"].ReadValue<float>() > 0.9f; //is the jump button being held down this frame?
        jumpTriggered = myPlayerInput.actions["Jump"].triggered; //was the jump button just pressed this frame?
        if(myPlayerInput.actions["Move"].ReadValue<Vector2>().x == 0){
            inputX = 0; 
            climbing = false;
        }
        if(inputX > 0) spriteRenderer.flipX = false;
        if(inputX < 0) spriteRenderer.flipX = true;

        //if I just pressed the jump button
        if (jumpTriggered)
        {
            lastJumpPressedTimer = lastJumpPressedTimerMax; 
            if (!onPlatform) { //if I'm in the air
                if (coyoteTimer > 0) BeginJump(); //if I just left the ground, I have a few milliseconds to still jump
                else {
                    jumpPreloadTimer = jumpPreloadTimerMax; //if I'm in the air and I press the jump button, preload it so I can jump if I land very soon
                }
            }else if(climbing){ //if on wall
                jumping = true;
            }
            else { //if I'm on the ground, being a jump
                BeginJump();
            }
            
        }

        //lower these timers at the end of each frame
        jumpPreloadTimer -= Time.deltaTime;
        coyoteTimer -= Time.deltaTime;
        airTimer -= Time.deltaTime;
        lastJumpPressedTimer -= Time.deltaTime;
    }



    //all our physics logic is in fixedupdate
    private void FixedUpdate()
    {
        //if I'm jumping and the jump button is still held down, don't apply any gravity and increase the jumpHeldTimer
        if (jumping && jumpHeldTimer < jumpHeldTimerMax) {
            if (jumpHeld) jumpHeldTimer += Time.fixedDeltaTime;
            else jumpHeldTimer = jumpHeldTimerMax;
        }else if(climbing && airTimer > 0){ //if climbing, no fall
            velocity.y = 0;
        }else if (apexReached && apexTimer > 0) {//if I reached the apex, lower the gravity until the apexTimer runs out...
            apexTimer -= Time.fixedDeltaTime;
            velocity.y -= .3f * gravity * Time.fixedDeltaTime;
            //...and enable the footBoxCollider, which allows me to land on the edge of platforms
            if(apexTimer <=0) footBoxCollider.enabled = true;
        }else{ //not pushed, not jumping, apply normal gravity
            velocity.y -= gravity * Time.fixedDeltaTime;
            if (velocity.y < -fallSpeedMax) velocity.y = -fallSpeedMax; //make sure fall speed never exceeds fallSpeedMax
            //but also keep checking if I reached the apex, and set apex values if it's true
            if (!apexReached && jumping && velocity.y <= 0) {
                apexReached = true;
                apexTimer = apexTimerMax;
            }
        }

        if(climbing){
            if(lastJumpPressedTimer > 0){ //wall jump x
                Debug.Log("wall jump");
                velocity.y = jumpStartingPush * wallJumpScalerY;
                velocity.x = -inputX * jumpStartingPush * wallJumpScalerX;
                apexReached = true;
                wallJumping = true;
            }else if (wallJumping){
                velocity.x -= 1.2f * inputX * gravity * Time.fixedDeltaTime;
            }else {
                velocity.y = inputY * speed * Time.fixedDeltaTime;
            }
        }else{ //x movement on platform
            velocity.x = inputX * speed * Time.fixedDeltaTime;
        }

        //move to the next position while checking all spaces between the current position and the next position
        rb.MovePosition(rb.position + velocity);
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Platform") {
            /*
            When you enter a platform, set this value to true so that when you are on top of a platform, jump will get reset.
            This is here for an edge case where you enter a platform by sliding up its side but then land on top of the platform without ever leaving it.
            If we just reset the jump here, then it would get reset when you touch a wall. 
            But if we only reset when you land directly on top of a platform, then it wouldn't reset the jump when you slide up a wall onto the top of a platform.
            */
            enteredPlatformJumpNotResetYet = true;
            wallJumping = false;
            if(jumping) StartCoroutine(cameraFollow.shakeCorutine(0.03f, 0.1f, 0.1f, 0.5f));
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Platform") //if the object I am touching is a platform
        {
            foreach(ContactPoint2D contact in collision.contacts) //go through each contact point where I collided with this platform
            {
                //draw a debug ray in the Unity scene view that shows the normal of the point where I collided with the platform in Unity (not visible in play mode)
                Debug.DrawRay(contact.point, contact.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 1f);

                //am I standing on the platform vertically?
                if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x))
                {
                    climbing = false; //cannot climb if on platform
                    apexReached = false;
                    if (contact.normal.y > 0) //am I standing on top of the platform?
                    {
                        onPlatform = true;
                        if (velocity.y < 0) velocity.y = 0; //if I landed on the platform, make sure my velocity is not negative
                        if(jumpPreloadTimer > 0) BeginJump(); //if I pressed the jump button just before landing, jump now
                        if(enteredPlatformJumpNotResetYet) { //see the long comment in OnCollisionEnter2D for why this is here
                            ResetJump();
                            enteredPlatformJumpNotResetYet = false;
                        }
                    }
                    else //am I touching the platform from below?
                    {
                        if (velocity.y > 0) velocity.y = 0; //if I bumped my head below the platform, make sure my velocity is no longer positive
                    }
                }
                else //did I hit the platform from the side?
                {  
                    //wall climbing
                    if(velocity.y != 0 && !climbing &&
                    ((contact.normal.x > 0 && inputX < 0)||(contact.normal.x < 0 && inputX > 0))){ //hold buttons when hitting
                        climbing = true;
                        jumping = false;
                    }
                }

            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.tag == "Platform") //if the object I just stopped touching is a platform
        {
            if (onPlatform) {
                onPlatform = false; //I'm no longer on a platform
                airTimer = wallTimerMax; 
                coyoteTimer = coyoteTimerMax; //start the coyote timer so I can still jump for a few milliseconds after leaving the platform
                enteredPlatformJumpNotResetYet = false; //see the long comment in OnCollisionEnter2D for how enteredPlatformJumpNotResetYet works
            }
            if(climbing) {
                climbing = false;
            }
        }
    }



    /// <summary>
    /// Begins a new jump.Sets velocity.y to jumpStartingPush, sets jumping to true, and sets jumpHeldTimer to 0. Disables footBoxCollider.
    /// </summary>
    void BeginJump() {
        velocity.y = jumpStartingPush;
        jumping = true;
        jumpHeldTimer = 0;
        footBoxCollider.enabled = false;
        airTimer = wallTimerMax;
    }

    /// <summary>
    /// Call this when a jump ends. Resets jumping and apexReached to false.
    /// </summary>
    void ResetJump() {
        jumping = false;
        apexReached = false;
    }
}
