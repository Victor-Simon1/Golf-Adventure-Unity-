using System;
using Services;
using UnityEngine;

public class HoleBehavior : MonoBehaviour, IComparable
{
    public int id;
    public static int max;

    private void OnEnable()
    {
        max += 1;
        Debug.Log("Register Hole " + id);

        ServiceLocator.Get<GameManager>().AddHole(this);
    }
    public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as HoleBehavior;

        if (a.id < b.id)
            return -1;

        if (a.id > b.id)
            return 1;

        return 0;
    }


}
