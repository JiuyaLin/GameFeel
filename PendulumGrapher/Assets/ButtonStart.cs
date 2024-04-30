using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        if(Input.GetKeyDown(KeyCode.Space)){
            startPendulum();
        }
    }

    void startPendulum(){
        Penduluim pendScript = GetComponent<Penduluim>();
        if(!pendScript.enabled){
            pendScript.enabled = true;
        }
    }
}
