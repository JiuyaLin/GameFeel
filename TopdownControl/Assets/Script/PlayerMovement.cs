using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 velocity;
    public float speed;
    public List<Transform> wallList = new List<Transform>();
    
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

        velocity = velocity.normalized * speed;
        
        List<Vector3> cornerList = new List<Vector3>();
        
        Vector3 nextPosition = transform.position + velocity * Time.deltaTime;


        Vector3 topRightCorner = new Vector3(nextPosition.x + transform.localScale.x / 2, nextPosition.y + transform.localScale.y / 2, nextPosition.z);
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
        }

        transform.position = nextPosition;
    }
}
