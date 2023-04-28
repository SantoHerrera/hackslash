using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            Debug.Log("UpArrow press...");
        }
        if(Input.GetKeyDown(KeyCode.DownArrow)) 
        {
            Debug.Log("DownArrow press...");
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)) 
        {
            
            
            Debug.Log("RightArrow press...");
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            Debug.Log("LeftArrow press...");
        }

        // if(Input.GetKeyUp(KeyCode.UpArrow))
        // {
        //     Debug.Log(".... End space press");
        // }
    }
}
