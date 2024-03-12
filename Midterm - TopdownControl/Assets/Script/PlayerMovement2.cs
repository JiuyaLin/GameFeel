using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using Quaternion = UnityEngine.Quaternion;
using Unity.VisualScripting;

public class PlayerMovement2 : MonoBehaviour
{
    Vector3 velocityThisFrame, velocity, acceleration, microVelocity, positionNextFrame, bounceAim, 
        spriteSize, spriteScaler, bursLocation, startPos;
    private float particleTimer, burstParticleCounter, burstCount, spritePosX, spritePosY, bounceSafeTimer;
    private bool collidingApplied, bouncingApplied;
    private char curKey;
    private string particleBurstType;
    public static float trincketCollected, timer;
    public GameObject particle, playerSprite;
    public List<Transform> wallList = new List<Transform>();
    public List<GameObject> trincketList = new List<GameObject>();
    private SpriteRenderer pcSpriteRenderer;
    private Vector2[] boundCorners;
    [SerializeField] float movementIncrement, accelerationSpeed, maxSpeed, friction, camFlipTime,
        counterAccelerationModifier, bounceFactor, breakFactor;
    [SerializeField] Vector2 particleTimerRange;
    public AudioClip[] audioclips;
    public AudioSource audioSource;

    
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        pcSpriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        spriteSize.x = pcSpriteRenderer.bounds.size.x;
        spriteSize.y = pcSpriteRenderer.bounds.size.y;
        particleTimer = Random.Range(particleTimerRange.x, particleTimerRange.y);
        burstCount = 25;//total number of particles
        bounceSafeTimer = 0.1f;
        burstParticleCounter = burstCount; 
        collidingApplied = false;
        bouncingApplied = false;
        spriteScaler = new Vector3(1, 1, 1);
        curKey = ' '; //current key being pressed
        findBoundCorners();
        audioSource = GetComponent<AudioSource>();

        //add all ways to the wallList
        string[] wallTagNames = {"WallBounce", "WallBreak", "NeutralWall"};
        foreach(string tagName in wallTagNames){
            GameObject[] tempWalls1 = GameObject.FindGameObjectsWithTag(tagName);
            foreach(GameObject wall in tempWalls1){
                wallList.Add(wall.transform);
            }
        }
        trincketList.AddRange(GameObject.FindGameObjectsWithTag("Trincket"));
        Debug.Log("Trincket Count: " + trincketList.Count);

    }

    // Update is called once per frame
    void Update()
    {
        float angleVelocityVsAcceleration = Vector3.Angle(velocity, acceleration);
        float counterPushRatio = angleVelocityVsAcceleration / 180;
        acceleration = Vector3.zero; //zero out the acceleration at the beginning of each frame
        //velocity = Vector3.zero;
        //using get keydown/up to eliminate hierarchy of keys
        if(!bouncingApplied){
            if(Input.GetKeyDown(KeyCode.W)) curKey = 'W';
            if(Input.GetKeyDown(KeyCode.S)) curKey = 'S';
            if(Input.GetKeyDown(KeyCode.D)) curKey = 'D';
            if(Input.GetKeyDown(KeyCode.A)) curKey = 'A';
            if(Input.anyKeyDown) audioSource.PlayOneShot(audioclips[3]);
            if(curKey == 'W') acceleration.y += 1 ; //W moves up
            if(curKey == 'S') acceleration.y -= 1 ; //S moves down
            if(curKey == 'D') acceleration.x += 1 ; //D moves right
            if(curKey == 'A') acceleration.x -= 1 ; //A moves left
        }
        if(Input.GetKeyUp(KeyCode.W) && curKey == 'W'
        || Input.GetKeyUp(KeyCode.A) && curKey == 'A'
        || Input.GetKeyUp(KeyCode.S) && curKey == 'S'
        || Input.GetKeyUp(KeyCode.D) && curKey == 'D') curKey = ' ';
        if(Input.GetKeyDown(KeyCode.R)) transform.position = startPos;//restart

        acceleration = acceleration.normalized * accelerationSpeed * Time.deltaTime;
        velocity += acceleration * (1 + counterPushRatio * counterAccelerationModifier);
        if(acceleration == Vector3.zero){
            if(velocity.magnitude > friction * Time.deltaTime){
                velocity -= velocity.normalized * friction * Time.deltaTime;
            }else{
                velocity = Vector3.zero;
            }
        }

        velocityThisFrame = velocity;
        positionNextFrame = transform.position;
        bool velXPositive = false;
        if(velocityThisFrame.x > 0) velXPositive = true;
        bool velYPositive = false;
        if(velocityThisFrame.y > 0) velYPositive = true;
        if((bounceAim - positionNextFrame).magnitude < 0.1f) {
            bouncingApplied = false; 
            bounceAim = positionNextFrame;
        }
        if(bouncingApplied){
            positionNextFrame = Vector3.Lerp(positionNextFrame, bounceAim, 0.5f);
            if(isThisPositionInWall(positionNextFrame)) positionNextFrame = transform.position;
        }

        //check walls
        while(velocityThisFrame != Vector3.zero){ //when moving
            if(Mathf.Abs(velocityThisFrame.x)> Mathf.Abs(velocityThisFrame.y)){
                velocityThisFrame.x -= MovementIncrementSigned(velXPositive);
                microVelocity = new Vector3(MovementIncrementSigned(velXPositive), 0, 0);
                if(isThisPositionInWall(positionNextFrame + microVelocity)){
                    velocityThisFrame.x = 0;
                    if(!collidingApplied){
                        applyEffect();
                        collidingApplied = true;
                        Debug.Log("applied effect");
                    }
                }else{ 
                    positionNextFrame += microVelocity;
                    collidingApplied = false;
                }

                if(velocityThisFrame.x < movementIncrement && velocityThisFrame.x > -movementIncrement){
                    velocityThisFrame.x = 0;
                }
            }else{//if velocity y > velocity x
                velocityThisFrame.y -= MovementIncrementSigned(velYPositive);
                microVelocity = new Vector3(0, MovementIncrementSigned(velYPositive), 0);
                if(isThisPositionInWall(positionNextFrame + microVelocity)){
                    velocityThisFrame.y = 0;
                    if(!collidingApplied){
                        applyEffect();
                        collidingApplied = true;
                        Debug.Log("applied effect");
                    }
                }else{ 
                    positionNextFrame += microVelocity;
                    collidingApplied = false;
                }

                if(velocityThisFrame.y < movementIncrement && velocityThisFrame.y > -movementIncrement){
                    velocityThisFrame.y = 0;
                }
            }
            //particle
            GameObject newParticle;
            if(particleTimer <= 0){
                particleTimer = Random.Range(particleTimerRange.x, particleTimerRange.y);
                newParticle = Instantiate(particle, positionNextFrame, Quaternion.identity);
                newParticle.GetComponent<Particle>().particleType = "Move";
            }
            if(burstParticleCounter > 0){
                newParticle = Instantiate(particle, bursLocation, Quaternion.identity);
                burstParticleCounter--;
                if(particleBurstType == "Break"){ // break gives green particles
                    newParticle.GetComponent<Particle>().particleType = "Break";
                }else if(particleBurstType == "Bounce"){ // bounces gives blue particles
                    newParticle.GetComponent<Particle>().particleType = "Bounce";
                }
            }
        }//when moving

        //sprites
        spritePosX = 0; spritePosY = 0;
        spriteScaler = new Vector3(1, 1, 1);
        float scaler = 1 - 0.35f * velocity.magnitude / maxSpeed;
        if(Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x)){
            spriteScaler.y = scaler;
            if(velocity.y > 0) spritePosY += spriteSize.y/2 * (1 - scaler);
            else spritePosY -= spriteSize.y/2 * (1 - scaler);
        }else{
            spriteScaler.x = scaler;
            if(velocity.x > 0) spritePosX += spriteSize.x/2 * (1 - scaler);
            else spritePosX -= spriteSize.x/2 * (1 - scaler);
        }
        playerSprite.transform.localScale = spriteScaler;
        playerSprite.transform.localPosition = new Vector3(spritePosX, spritePosY, 0);
        collectTrcinket(); //check trincket

        //Move player, finalized velocity
        if(velocity.magnitude > maxSpeed) velocity = velocity.normalized * maxSpeed;
        transform.position = positionNextFrame;
        particleTimer -= Time.deltaTime;
        timer += Time.deltaTime;
        bounceSafeTimer -= Time.deltaTime;
        if(bounceSafeTimer < 0) bouncingApplied = false;
    }


    float MovementIncrementSigned(bool positiveDirection){
        if(positiveDirection) return movementIncrement;
        else return -movementIncrement;
    }

    bool isThisPositionInWall(Vector3 positionToCheck){ //true if it is a wall
        //New Method
        foreach(Transform currentWall in wallList){
            if(currentWall != null){
                float xDist = Mathf.Abs(positionToCheck.x - currentWall.position.x);
                float yDist = Mathf.Abs(positionToCheck.y - currentWall.position.y);
                float xMaxDist = transform.localScale.x /2 + currentWall.localScale.x/2;
                float yMaxDist = transform.localScale.y /2 + currentWall.localScale.y/2;
                if(xDist < xMaxDist && yDist < yMaxDist){
                    return true;
                }
            }
            
            
        }
        return false;
    }

    void applyEffect(){
        GameObject closestWall = findClosetWall();
        if(closestWall != null && closestWall.tag == "WallBreak" 
        && velocity.magnitude > maxSpeed * breakFactor){
            Debug.Log("Break Wall");
            bouncingApplied = false;
            wallList.Remove(closestWall.transform); //remove from list
            Destroy(closestWall);
            bursLocation = positionNextFrame;
            burstParticleCounter = burstCount;
            particleBurstType = "Break";
            audioSource.PlayOneShot(audioclips[1]);
        }else if(closestWall != null && closestWall.tag == "WallBounce" &&
        bounceSafeTimer < 0){
            Debug.Log("Bounce Wall");
            bounceSafeTimer = 0.1f;
            bounceAim = positionNextFrame;
            if(Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y)){
                bounceAim.x -= velocity.x * Time.deltaTime * bounceFactor;
                velocity.x *= -1;
            }else{
                bounceAim.y -= velocity.y  * Time.deltaTime * bounceFactor;
                velocity.y *= -1;
            } 
            bouncingApplied = true;
            bursLocation = positionNextFrame;
            burstParticleCounter = burstCount;
            particleBurstType = "Bounce";
            audioSource.PlayOneShot(audioclips[4]);
        }
    }

    GameObject findClosetWall(){
        float closestDistance = Mathf.Infinity;
        GameObject closestWall = null;
        foreach(Transform currentWall in wallList){
            if(currentWall == null) continue; //skip if null (destroyed wall
            float distance = Vector3.Distance(transform.position, currentWall.position);
            if(distance < closestDistance){
                closestDistance = distance;
                closestWall = currentWall.gameObject;
            }
        }
        //special case: check the disance to bounds
        float towall = Mathf.Min(Mathf.Abs(transform.position.x - boundCorners[0].x), 
                                Mathf.Abs(transform.position.x - boundCorners[1].x), 
                                Mathf.Abs(transform.position.y - boundCorners[1].y),
                                Mathf.Abs(transform.position.y - boundCorners[2].y));
        if(towall < closestDistance){
            closestWall = null;
            Debug.Log("Close to bound");
        }
        return closestWall;
    }

    void findBoundCorners(){
        List<Transform> wallBoundsLoc = new List<Transform>();
        GameObject[] wallBounds = GameObject.FindGameObjectsWithTag("NeutralWall");
        foreach(GameObject wall in wallBounds) wallBoundsLoc.Add(wall.transform);
        //find the corners of the bounds
        float xMax = Mathf.NegativeInfinity, xMin = Mathf.Infinity, yMax = Mathf.NegativeInfinity, yMin = Mathf.Infinity;
        foreach(Transform wall in wallBoundsLoc){
            if(wall.position.x > xMax) xMax = wall.position.x;
            if(wall.position.x < xMin) xMin = wall.position.x;
            if(wall.position.y > yMax) yMax = wall.position.y;
            if(wall.position.y < yMin) yMin = wall.position.y;
        }
        //all walls same size
        float Hwallthickness = Mathf.Min(wallBoundsLoc[0].localScale.x, wallBoundsLoc[0].localScale.y);
        boundCorners = new Vector2[4];
        boundCorners[0] = new Vector2(xMin - Hwallthickness, yMin - Hwallthickness);
        boundCorners[1] = new Vector2(xMax + Hwallthickness, yMin - Hwallthickness);
        boundCorners[2] = new Vector2(xMax + Hwallthickness, yMax + Hwallthickness);
        boundCorners[3] = new Vector2(xMin - Hwallthickness, yMax + Hwallthickness);
    }

    void collectTrcinket(){
        foreach(GameObject thisTrincket in trincketList){
            if((transform.position - thisTrincket.transform.position).magnitude < 1.5f){
                trincketCollected++;
                trincketList.Remove(thisTrincket);
                for(int i = 0; i < 25; i++){
                    GameObject newParticle = Instantiate(particle, thisTrincket.transform.position, Quaternion.identity);
                    newParticle.GetComponent<Particle>().particleType = "TrincketBurst";
                }
                audioSource.PlayOneShot(audioclips[2]);
                Destroy(thisTrincket); break;
            }
        Debug.Log("Trincket Location: " + (transform.position - thisTrincket.transform.position).magnitude);
        }
    }

}
