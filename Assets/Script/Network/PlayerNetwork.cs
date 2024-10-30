using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Services;

public class PlayerNetwork : NetworkBehaviour
{

    [SerializeField] Behaviour[] componentsToDisable;
    private int netID;
    private JoinManager joinManager;

#region UNITY_FUNCTION
    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) 
        { 
            for(int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }    
    }

#endregion

#region MIRROR_FUNCTION
    public override void OnStartClient()
    {
        Debug.Log("Player Network OnStartClient");
        base.OnStartClient();

        joinManager = ServiceLocator.Get<JoinManager>();

        joinManager.Connected();
        netID = (int) transform.parent.parent.GetComponent<NetworkIdentity>().netId;

        transform.parent.name = "Player " + netId;

        PlayerController player = transform.parent.parent.GetComponent<PlayerController>();

        player.id = netID - 1;

        var gm = ServiceLocator.Get<GameManager>();

        gm.RegisterPlayer(player);

        CmdUpdateUI(gm.GetListPlayer());

        CmdUpdatePlayerName(player.id, joinManager.GetPlayerName() + player.id.ToString());

    }

#endregion

#region CMD
    [Command]
    private void CmdUpdatePlayerName(int id, string username)
    {
        PlayerController player = ServiceLocator.Get<GameManager>().GetPlayer(id);
        if(player != null)
        {
            Debug.Log(username + " has joined !");
            player.SetName(username);
        }
    }

    [Command]
    private void CmdUpdateUI(List<PlayerController> pcs)
    {
        var gm = ServiceLocator.Get<GameManager>();
        RpcUpdateTitle(gm.GetSessionName());
        RpcUpdateDisplay(pcs);
    }
#endregion
 
#region RPC
    [ClientRpc]
    private void RpcUpdateTitle(string sessionName)
    {
        ServiceLocator.Get<HubUIManager>().SetSessionNameWithoutNotify(sessionName);
    }

    [ClientRpc]
    private void RpcUpdateDisplay(List<PlayerController> pcs)
    {
        ServiceLocator.Get<HubUIManager>().UpdatePlayers(pcs);
    }

#endregion
    /*

        //Désactiver les go importants(camera principale)
        private void OnDisable()
        {
            GameManager.UnregisterPlayer(transform.name);
        }
    */
}
