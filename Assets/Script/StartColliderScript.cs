using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Services;
using System;

public class StartColliderScript : MonoBehaviour, IComparable
{
    public int id;
    public static int max;

    private void OnEnable()
    {
        max += 1;
        Debug.Log("Register Hole " + id);

        ServiceLocator.Get<GameManager>().AddStart(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        BallControler bc= other.GetComponent<BallControler>();
        if(bc != null)
        {
            bc.IgnoreBalls();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        BallControler bc= other.GetComponent<BallControler>();
        if(bc != null)
        {
            bc.DontIgnoreBalls();
        }
    }

    public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as StartColliderScript;

        if (a.id < b.id)
            return -1;

        if (a.id > b.id)
            return 1;

        return 0;
    }
}
