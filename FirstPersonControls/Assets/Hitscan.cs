using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hitscan : MonoBehaviour
{
    PlayerInput playerInput;
    Transform cameraHolder;
    bool hitscanTriggered;
    [SerializeField] LayerMask hitscanLayerMask;
    [SerializeField] float aimAssitLevel;
    [SerializeField] GameObject hitscanVisual;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        cameraHolder = transform.Find("Camera Holder");
    }

    // Update is called once per frame
    void Update()
    {
        hitscanTriggered = playerInput.actions["Hitscan"].triggered;

        if(hitscanTriggered){
            //List<GameObject> hitscanHits = new List<GameObject>();
            Instantiate(hitscanVisual, cameraHolder.position + (cameraHolder.forward * 150) - (cameraHolder.up * .5f),
            cameraHolder.rotation, null);

            if(HitscanRaycast(cameraHolder.position, cameraHolder.forward) == false){
                HitscanSpherecast(cameraHolder.position, cameraHolder.forward);
            }
        }


    }

    bool HitscanRaycast(Vector3 origin, Vector3 direction){
        if(direction == null) direction = cameraHolder.forward;
        RaycastHit hit;
        if(Physics.Raycast(origin, direction, out hit, Mathf.Infinity, hitscanLayerMask)){
            return HitscanEvaluateObjectHit(hit);
        }
    
        return false;
    }

    bool HitscanSpherecast(Vector3 origin, Vector3 direction){
        if(direction == null) direction = cameraHolder.forward;
        RaycastHit hit;
        if(Physics.Raycast(origin, aimAssitLevel * direction, out hit, Mathf.Infinity, hitscanLayerMask)){
            return HitscanEvaluateObjectHit(hit);
        }
    
        return false;
    }

    bool HitscanEvaluateObjectHit(RaycastHit hit){
        if(hit.collider.gameObject.tag == "Enemytarget"){
            Debug.Log("Hit Enemy");
            Destroy(hit.collider.gameObject);
            InstantiateFruit.RangeKill++;
            InstantiateFruit.totalFruitNum--;
            return true;
        }
        return false;
    }
}
