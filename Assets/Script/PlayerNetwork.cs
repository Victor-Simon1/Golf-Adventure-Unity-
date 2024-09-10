using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Services;

public class PlayerNetwork : NetworkBehaviour
{

    [SerializeField] Behaviour[] componentsToDisable;
    private int netID;

    // Start is called before the first frame update
    void Start()
    {
        if(!isLocalPlayer) 
        { 
            for(int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }    
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        netID = (int) transform.parent.GetComponent<NetworkIdentity>().netId;
        PlayerController player = transform.parent.GetComponent<PlayerController>();
        ServiceLocator.Get<GameManager>().RegisterPlayer(netID,player);
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
