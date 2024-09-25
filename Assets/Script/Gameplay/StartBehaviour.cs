using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using System;

public class StartBehaviour : MonoBehaviour, IComparable
{

    public int id;
    public static int max;

    private void OnEnable()
    {
        max += 1;
        Debug.Log("Register Start " + id);

        ServiceLocator.Get<GameManager>().AddStart(this);
    }
    public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as StartBehaviour;

        if (a.id < b.id)
            return -1;

        if (a.id > b.id)
            return 1;

        return 0;
    }
}
