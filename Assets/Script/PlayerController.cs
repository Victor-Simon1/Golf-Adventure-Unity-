using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : NetworkBehaviour, IComparable
{
    /*[Client]
    [ClientRpc]
    [Command]*/
    public int strokes;

    [SyncVar]
    [SerializeField] private string playerName = "Player";

    public int id;


    [SerializeField] private GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ClientRpc]
    public void RpcAddStroke()
    {
        strokes++;
    }

    [ClientRpc]
    public void RpcFinishFirstPut()
    {
        GetComponent<SphereCollider>().excludeLayers = 0;
    }

    public void SpawnBall()
    {
        ball.SetActive(true);
    }

    public void DespawnBall()
    {
        ball.SetActive(false);
    }

    public void SetName(string newName)
    {
        this.playerName = newName;
    }

    public string GetName()
    {
        return playerName;
    }

    public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as PlayerController;

        if (a.id < b.id)
            return -1;

        if (a.id > b.id)
            return 1;

        return 0;
    }

}
