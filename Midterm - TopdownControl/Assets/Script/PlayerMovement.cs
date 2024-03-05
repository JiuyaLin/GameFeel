using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 velocity;
    public float speed;
    public List<Transform> wallList = new List<Transform>();
    private SpriteRenderer mySpriteRenderer;
    [SerializeField] float movementIncrement;
    
    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = Vector3.zero; //zeo out the velocity at the beginning of each frame

        if(Input.GetKey(KeyCode.W)) velocity.y += 1 ; //W moves up
        if(Input.GetKey(KeyCode.S)) velocity.y -= 1 ; //S moves down
        if(Input.GetKey(KeyCode.D)) velocity.x += 1 ; //D moves right
        if(Input.GetKey(KeyCode.A)) velocity.x -= 1 ; //A moves left

        velocity = velocity.normalized * speed * Time.deltaTime;
        Vector3 microVelocity;
        Vector3 positionNextFrame = transform.position;
        bool velXPositive = false;
        if(velocity.x > 0) velXPositive = true;
        bool velYPositive = false;
        if(velocity.y > 0) velYPositive = true;

        while(velocity != Vector3.zero){
            if(Mathf.Abs(velocity.x)> Mathf.Abs(velocity.y)){
                velocity.x -= MovementIncrementSigned(velXPositive);
                microVelocity = new Vector3(MovementIncrementSigned(velXPositive), 0, 0);
                if(isThisPositionInWall(positionNextFrame + microVelocity)){
                    velocity.x = 0;
                }else{ 
                    positionNextFrame += microVelocity;
                }

                if(velocity.x < movementIncrement && velocity.x > -movementIncrement){
                    velocity.x = 0;
                }
            }else{//if velocity y > velocity x
                velocity.y -= MovementIncrementSigned(velYPositive);
                microVelocity = new Vector3(0, MovementIncrementSigned(velYPositive), 0);
                if(isThisPositionInWall(positionNextFrame + microVelocity)){
                    velocity.y = 0;
                }else{ 
                    positionNextFrame += microVelocity;
                }

                if(velocity.y < movementIncrement && velocity.y > -movementIncrement){
                    velocity.y = 0;
                }
            }

            
        }

        transform.position = positionNextFrame;
        
        // List<Vector3> cornerList = new List<Vector3>();
        
        // if(isThisPositionInWall(transform.position)){
        //     Debug.Log("In Wall");
        // }
        // Vector3 nextPosition = transform.position + velocity * Time.deltaTime;


        /* Vector3 topRightCorner = new Vector3(nextPosition.x + transform.localScale.x / 2, nextPosition.y + transform.localScale.y / 2, nextPosition.z);
        Vector3 bottomRightCorner = new Vector3(nextPosition.x + transform.localScale.x / 2, nextPosition.y - transform.localScale.y / 2, nextPosition.z);
        Vector3 topLeftCorner = new Vector3(nextPosition.x - transform.localScale.x / 2, nextPosition.y + transform.localScale.y / 2, nextPosition.z);
        Vector3 bottomLeftCorner = new Vector3(nextPosition.x - transform.localScale.x / 2, nextPosition.y - transform.localScale.y / 2, nextPosition.z);

        cornerList.Add(topRightCorner);
        cornerList.Add(topLeftCorner);
        cornerList.Add(bottomLeftCorner);
        cornerList.Add(bottomRightCorner);


        // if(transform.position.x + (transform.localScale.x /2) > 4){
        //     Debug.Log("Off the square");
        //     nextPosition = transform.position;
        // }

        foreach(Transform currentWall in wallList)//do this for each wall in wall list
        {
            foreach(Vector3 currentCorner in cornerList)
            {
                if(currentCorner.x < currentWall.position.x + currentWall.localScale.x/2)
                {
                    if(currentCorner.x > currentWall.position.x - currentWall.localScale.x/2)
                    {
                        if(currentCorner.y < currentWall.position.y + currentWall.localScale.y/2)
                        {
                            if(currentCorner.y > currentWall.position.y - currentWall.localScale.y/2)
                            {
                                Debug.Log("Hi");
                                nextPosition = transform.position;
                            }
                        }
                    }
                }
            }
        }*/

        //transform.position = nextPosition;
    }

    // void SetColor(bool warningColor){
    //     if (warningColor) mySpriteRenderer.color = Color.red;
    //     else mySpriteRenderer.color = Color.white;
    // }

    float MovementIncrementSigned(bool positiveDirection){
        if(positiveDirection) return movementIncrement;
        else return -movementIncrement;
    }

    bool isThisPositionInWall(Vector3 positionToCheck){
        //New Method
        foreach(Transform currentWall in wallList){
            float xDist = Mathf.Abs(positionToCheck.x - currentWall.position.x);
            float yDist = Mathf.Abs(positionToCheck.y - currentWall.position.y);
            float xMaxDist = transform.localScale.x /2 + currentWall.localScale.x/2;
            float yMaxDist = transform.localScale.y /2 + currentWall.localScale.y/2;
            
            if(xDist < xMaxDist && yDist < yMaxDist){
                return true;
            }
        }
        return false;
    }
}
