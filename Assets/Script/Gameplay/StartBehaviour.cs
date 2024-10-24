using UnityEngine;
using Services;
using System;

public class StartBehaviour : MonoBehaviour, IComparable
{
    [Header("Variables")]
    public int id;
    public static int max;

    public GameManager gameManager;

#region UnityEditor
    private void Awake()
    {
        max += 1;
        Debug.Log("Register Start " + id);
    }

    private void Start()
    {
        gameManager = ServiceLocator.Get<GameManager>();
        gameManager.AddStart(this);
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

    private void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<BallControler>();
        if (ball != null)
        {
            if(ball.GetPlayer().isLocalPlayer)
            {
                ball.GetPlayer().hasArrived();
            }
        }
    }
#endregion
}
