using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerTrincket : MonoBehaviour
{
    public TextMeshProUGUI timerText, trincketText, successText;
    string timer, trincket;
    float totalTrincket, timerFixed;

    // Start is called before the first frame update
    void Start()
    {
        totalTrincket = GameObject.FindGameObjectsWithTag("Trincket").Length;
        timerFixed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer = "Time: " + Mathf.FloorToInt(PlayerMovement2.timer / 60) + ":" + Mathf.FloorToInt(PlayerMovement2.timer % 60);
        trincket = PlayerMovement2.trincketCollected + "/" + totalTrincket + " Trinckets";
        timerText.text = timer;
        if(PlayerMovement2.trincketCollected == totalTrincket){
            successText.text = "All Trinckets Collected!";
            if(timerFixed == 0) timerFixed = PlayerMovement2.timer;
            timerText.text = "Time: " + Mathf.FloorToInt(timerFixed / 60) + ":" + Mathf.FloorToInt(timerFixed % 60);;
        }
        trincketText.text = trincket;
    }
}
