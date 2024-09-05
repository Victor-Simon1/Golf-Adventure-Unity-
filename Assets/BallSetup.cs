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

    //D�sactiver les go importants(camera principale)
    private void OnDisable()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
