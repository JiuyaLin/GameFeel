using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerMovement : MonoBehaviour
{
    [SerializeField] Transform player;
    private Vector3 camCurLoc, camAimLoc, iniLocDifference;
    public Vector3 camRot;
    [SerializeField] float camLerpSpeed;

    // Start is called before the first frame update
    void Start()
    {
        iniLocDifference = transform.position - player.position;
    }

    // Update is called once per frame
    void Update()
    {   camAimLoc = player.position + iniLocDifference;
        camCurLoc = Vector3.Lerp(transform.position, camAimLoc, camLerpSpeed);

        transform.position = camCurLoc;
    }

}
