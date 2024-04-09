using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateFruit : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject spawnRange;
    Vector2 XRange, YRange, ZRange;
    public static int totalFruitNum, MelleKill, RangeKill;

    // Start is called before the first frame update
    void Start()
    {
        totalFruitNum = 99;
        MelleKill = 0; RangeKill = 0;
        XRange = new Vector2(spawnRange.transform.position.x - spawnRange.transform.localScale.x / 2, 
                spawnRange.transform.position.x + spawnRange.transform.localScale.x / 2);
        YRange = new Vector2(spawnRange.transform.position.y - spawnRange.transform.localScale.y / 2,
                spawnRange.transform.position.y + spawnRange.transform.localScale.y / 2);
        ZRange = new Vector2(spawnRange.transform.position.z - spawnRange.transform.localScale.z / 2,
                spawnRange.transform.position.z + spawnRange.transform.localScale.z / 2);
        for(int i = 0; i < totalFruitNum; i++){
            int randomIndex = Random.Range(0, prefabs.Length);
            Vector3 randomPosition = new Vector3(Random.Range(XRange.x, XRange.y),
                Random.Range(YRange.x, YRange.y), Random.Range(ZRange.x, ZRange.y));
            Instantiate(prefabs[randomIndex], randomPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
