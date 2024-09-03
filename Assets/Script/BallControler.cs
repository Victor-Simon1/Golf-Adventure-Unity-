using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;

public class BallControler : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Test
    public void Avance(InputAction.CallbackContext context)
    {
        //if (!base.Owner)
          //  return;
        transform.position += new Vector3(1, 0, 0);
        AvanceServer();
    }
    [ServerRpc]
    private void AvanceServer()
    {
        transform.position += new Vector3(1, 0, 0);
    }
}
