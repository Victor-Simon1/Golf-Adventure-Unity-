using UnityEngine;

public class StartColliderScript: MonoBehaviour
{
    #region UNITY_FUNCTION
    /* private void OnTriggerEnter(Collider other)
     {
         BallControler bc= other.GetComponent<BallControler>();
         if(bc != null)
         {
             bc.IgnoreBalls();
         }
     }*/
    private void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<BallControler>();
        if (ball != null)
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
        if(bc != null)
        {
            bc.DontIgnoreBalls();
        }
    }
#endregion
}
