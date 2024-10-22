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
