using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Penduluim : MonoBehaviour
{
    GameObject pendulum1, pendulum2;
    float g, LA, LAB;
    Vector3 uniZ;
    [SerializeField] float mA, mB;
    [SerializeField] Vector3 xA, xB, vA, vB, aA, aB;
    Vector3 FA, FB;
    List<Vector3> ASave, BSave;
    List<float> tSave;
    int clock;

    // Start is called before the first frame update
    void Start()
    {
        g = 9.81f;
        uniZ = new Vector3(0, 0, 1);
        pendulum1 = GameObject.Find("Pendulum1");
        pendulum2 = GameObject.Find("Pendulum2");
        Debug.Log(Time.fixedDeltaTime);
        FA = new Vector3(0, 0, -g * mA);
        FB = new Vector3(0, 0, -g * mB);
        LA = Vector3.Magnitude(xA);
        LAB = Vector3.Magnitude(xB-xA);
        ASave = new List<Vector3>();
        BSave = new List<Vector3>();
        tSave = new List<float>();
        vA = vA / 100;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Time.deltaTime is the time between update()
        float t = Time.deltaTime * clock;
        clock ++;

        //use A, B, vA, vB to find TA, TB
        float TB = mA * Vector3.Dot(vB, vB) - 2 * mA * Vector3.Dot(vA, vB) + mA * Vector3.Dot(vA, vA)
        + mA / LA * Vector3.Dot(xA, xB) * Vector3.Dot(vA, vB) -
         mA * Vector3.Dot(vA, vA) + 2 * mA * Vector3.Dot(FA, xB - xA) 
        + mA * Vector3.Dot(FB, xB - xA);
        float TA = mA / LA * Vector3.Dot(vA, vA) + mA / LA * Vector3.Dot(FA, xA) + 
        Vector3.Dot(xB - xA, xA) * TB / mA / LAB;

        //find aA, aB
        aA = TA * (- xA) / mA / LA + TB * (xB - xA) /mA / LAB + FA;
        aB = TB * (xA - xB)/mA / LAB + FB;
        //update vA, vB
        vA = vA + aA * Time.deltaTime;
        vB = vB + aB * Time.deltaTime;
        //update xA, xB
        xA = xA + vA * Time.deltaTime;
        xB = xB + vB * Time.deltaTime;

        ASave.Add(xA);
        BSave.Add(xB);
        tSave.Add(t);
        
        //move pendulum
        pendulum1.transform.position = xA;
        pendulum2.transform.position = xB;
    }
}
