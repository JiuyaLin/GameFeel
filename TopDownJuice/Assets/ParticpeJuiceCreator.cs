using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticpeJuiceCreator : MonoBehaviour
{
    [SerializeField] GameObject particleJuicePrefab;

    public void CreateParticles(int count){
        for(int i = 0; i < count; i++){
            GameObject newParticleJuice = Instantiate(particleJuicePrefab, transform.position, Quaternion.identity);
            newParticleJuice.transform.parent = transform;
        }
    }
}
