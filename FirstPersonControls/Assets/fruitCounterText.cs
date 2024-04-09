using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class fruitCounterText : MonoBehaviour
{
    TextMeshProUGUI textMesh;
    TextMeshProUGUI killCountMesh;
    string text;
    string killText;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        killCountMesh = GameObject.Find("Kill Counter").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text = "Fruit Remaining: " + InstantiateFruit.totalFruitNum;
        killText = "Melle Kill: " + InstantiateFruit.MelleKill + " Range Kill: " + InstantiateFruit.RangeKill;
        textMesh.text = text;
        killCountMesh.text = killText;
    }
}
