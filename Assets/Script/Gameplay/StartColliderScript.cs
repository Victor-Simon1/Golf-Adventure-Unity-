using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Services;
using System;

public class StartColliderScript: MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        /*BallControler bc= other.GetComponent<BallControler>();
        if(bc != null)
        {
            bc.IgnoreBalls();
        }*/
    }
    
    private void OnTriggerExit(Collider other)
    {
        BallControler bc= other.GetComponent<BallControler>();
        if(bc != null)
        {
            bc.DontIgnoreBalls();
        }
    }
}
