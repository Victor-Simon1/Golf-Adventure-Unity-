using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class BallSetup : NetworkBehaviour
{

    [SerializeField] Behaviour[] componentsToDisable;

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

        string netID = transform.parent.GetComponent<NetworkIdentity>().netId.ToString();
        Player player = transform.parent.GetComponent<Player>();
        GameManager.RegisterPlayer(netID,player);
    }

    
    //Désactiver les go importants(camera principale)
    private void OnDisable()
    {
        GameManager.UnregisterPlayer(transform.name);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
