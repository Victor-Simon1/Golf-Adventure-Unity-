using Mirror;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private PlayerDisplay display;

    private Material mat;

    private void Start()
    {
        mat = new Material(Shader.Find("Standard"));
        mat.SetFloat("_Glossiness", .8f);
        mat.SetFloat("_Metallic", 0f);

        GetComponent<Renderer>().material = mat;
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

    [Command]
    public void CmdExit()
    {
        RpcExit();
        PlayerDestroy();
    }

    [ClientRpc]
    public void RpcExit()
    {
        var gm = ServiceLocator.Get<GameManager>();
        gm.StopConnection();
        PlayerDestroy();
    }

    [Command]
    public void CmdStopHost()
    {
        RpcStopHost();
    }

    [ClientRpc]
    public void RpcStopHost()
    {
        ServiceLocator.Get<GameManager>().ThrowError("Vous avez été déconnecté du serveur.");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isLocalPlayer)
        {
            ServiceLocator.Get<GameManager>().ThrowError("Vous avez été déconnecté du serveur.");
        }
    }

    [ClientRpc]
    public void RpcLaunch(string mapId)
    {
        if(isLocalPlayer)
        {
            SceneManager.LoadScene(mapId);
        }
    }

    private void PlayerDestroy()
    {
        Destroy(display.gameObject);
        Destroy(gameObject);
    }

    public void SpawnBall()
    {
        ball.SetActive(true);
    }

    public void DespawnBall()
    {
        ball.SetActive(false);
    }

    public void SetColor(Color color)
    {
        mat.color = color;
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

    public void SetDisplay(PlayerDisplay display)
    {
        this.display = display;
    }
}
