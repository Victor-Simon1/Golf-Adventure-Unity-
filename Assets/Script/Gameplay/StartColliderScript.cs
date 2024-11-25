using System.Collections.Generic;
using UnityEngine;
using Services;

public class StartColliderScript: MonoBehaviour
{
    private bool[] ArrayBallExit;

    #region UNITY_FUNCTION
    /* private void OnTriggerEnter(Collider other)
     {
         BallControler bc= other.GetComponent<BallControler>();
         if(bc != null)
         {
             bc.IgnoreBalls();
         }
     }*/

    private void Awake()
    {
        ArrayBallExit = new bool[ServiceLocator.Get<GameManager>().GetListPlayer().Count];
    }

    private void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<BallControler>();
        if (ball != null && !ArrayBallExit[ball.GetPlayer().id])
        {
            if (ball.GetPlayer().isLocalPlayer)
            {
                ball.GetPlayer().hasArrived();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        BallControler bc= other.GetComponent<BallControler>();
        if(bc != null && bc.moving)
        {
            ArrayBallExit[bc.GetPlayer().id] = true;
            bc.DontIgnoreBalls();
        }
    }
#endregion
}
