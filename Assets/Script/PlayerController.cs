using Mirror;
using Mirror.Examples.Pong;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
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
    private PlayerUI playerUI;

    private Material mat;

    private void Start()
    {
        mat = new Material(Shader.Find("Standard"));
        mat.SetFloat("_Glossiness", .8f);
        mat.SetFloat("_Metallic", 0f);

        ball.GetComponent<Renderer>().material = mat;
    }

    private void Update()
    {
        if(playerUI != null)
        {
            playerUI.SetStrokes(strokes);
        }
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
            ServiceLocator.Get<GameManager>().inGame = true;
            SceneManager.LoadScene(mapId);
            //SpawnBall();
        }
    }

    private void PlayerDestroy()
    {
        Destroy(display.gameObject);
        Destroy(gameObject);
    }

    public void TpToLocation(Transform location)
    {
        Debug.Log("tp to " + location.position);
        transform.position = location.position;
        //transform.position = new Vector3(location.position.x, location.position.y, location.position.z + id);
        SpawnBall();
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
        CmdSetColor(color, id);
    }
    [Command]
    public void PushBall(Vector3 dir,float force)
    {
        ball.GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.Impulse);
    }
    [Command]
    public void CmdSetColor(Color color,int id)
    {
        ServiceLocator.Get<GameManager>().SetPlayerColor(color,id);
    }

    [ClientRpc]
    public void RpcSetColor(Color color)
    {
        display.SetColor(color);
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

    public void SetPlayerUI(PlayerUI playerUI)
    {
        this.playerUI = playerUI;
    }
}
