using Mirror;
using Services;
using UnityEngine;

public class ReturnToHub : MonoBehaviour
{

#region PUBLIC_FUNCTION
    public void ExitGame()
    {
        GameManager gm = ServiceLocator.Get<GameManager>(true);
        NetworkManager nm = ServiceLocator.Get<StockNetManager>().GetNetworkManager();
        if (gm.IsHost()) 
        {
            //Debug.Log("Je suis l'hote");
            nm.StopHost();
        }
        else
        {
            //Debug.Log("Je suis client");
            nm.StopClient();
        }
    }
#endregion 
}
