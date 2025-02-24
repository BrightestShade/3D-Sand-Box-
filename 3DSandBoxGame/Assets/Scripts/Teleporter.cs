using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{

    
   
    private void OnTriggerEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            
        }
    }
}
