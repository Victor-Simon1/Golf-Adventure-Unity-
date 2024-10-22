using UnityEngine;
using Services;
using System;

public class StartBehaviour : MonoBehaviour, IComparable
{
    [Header("Variables")]
    public int id;
    public static int max;

#region UNITY_FUNCTION

    private void Awake()
    {
        max += 1;
        Debug.Log("Register Start " + id);
    }

    private void Start()
    {
        ServiceLocator.Get<GameManager>().AddStart(this);
    }

#endregion

#region ICOMPARABLE_FUNCTION
    //Function of comparison between two StartBehavior
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
#endregion
}
