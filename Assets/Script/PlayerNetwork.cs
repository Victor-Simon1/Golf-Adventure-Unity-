using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Services;

public class PlayerNetwork : NetworkBehaviour
{

    [SerializeField] Behaviour[] componentsToDisable;
    private int netID;
    private JoinManager joinManager;

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

    public override void OnStartClient()
    {
        Debug.Log("Player Network OnStartClient");
        base.OnStartClient();

        netID = (int) transform.parent.GetComponent<NetworkIdentity>().netId;

        transform.name = "Player " + netId;

        PlayerController player = transform.parent.GetComponent<PlayerController>();

        player.id = netID - 1;

        var gm = ServiceLocator.Get<GameManager>();

        gm.RegisterPlayer(player);

        CmdUpdateUI(gm.GetSessionName(), gm.GetListPlayer());

        joinManager = ServiceLocator.Get<JoinManager>();


        CmdUpdatePlayerName(player.id, joinManager.GetPlayerName());

    }

    [Command]
    private void CmdUpdatePlayerName(int id, string username)
    {
        PlayerController player = ServiceLocator.Get<GameManager>().GetPlayer(id);
        if(player != null)
        {
            Debug.Log(username + " has joined !");
            player.SetName(username);
        }
        Debug.Log("fin update name");
    }

    [Command]
    private void CmdUpdateUI(string sessionName, List<PlayerController> pcs)
    {
        RpcUpdateTitle(sessionName);
        RpcUpdateDisplay(pcs);

        Debug.Log("fin update ui");
    }

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

/*
    
    //Désactiver les go importants(camera principale)
    private void OnDisable()
    {
        GameManager.UnregisterPlayer(transform.name);
    }
*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
