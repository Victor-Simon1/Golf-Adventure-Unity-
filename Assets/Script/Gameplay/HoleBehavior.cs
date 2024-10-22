using System;
using Services;
using UnityEngine;

public class HoleBehavior : MonoBehaviour, IComparable
{
    [Header("Variables")]
    public int id;
    public static int max;
    public int maxStrokes = 0;
 
#region UNITY_FUNCTION
    private void Awake()
    {
        max += 1;
        Debug.Log("Register Hole " + id);
    }

    private void Start()
    {
        ServiceLocator.Get<GameManager>().AddHole(this);
    }

#endregion

#region ICOMPARABLE_FUNCTION
    //Function of comparison between two Holebehavior object
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
#endregion

}
