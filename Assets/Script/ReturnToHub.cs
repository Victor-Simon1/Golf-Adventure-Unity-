using Mirror;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToHub : MonoBehaviour
{

    public void ExitGame()
    {
        GameManager gm = ServiceLocator.Get<GameManager>(true);
        NetworkManager nm = ServiceLocator.Get<StockNetManager>().GetNetworkManager();
        if (gm.IsHost()) 
        {
            Debug.Log("Je suis l'hote");
            nm.StopHost();
        }
        else
        {
            Debug.Log("Je suis client");
            nm.StopClient();
        }
    }
}
