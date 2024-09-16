using Mirror;
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

    private Material mat;

    private void Start()
    {
        mat = new Material(Shader.Find("Standard"));
        mat.SetFloat("_Glossiness", .8f);
        mat.SetFloat("_Metallic", 0f);

        ball.GetComponent<Renderer>().material = mat;
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
        ServiceLocator.Get<GameManager>().ThrowError("Vous avez �t� d�connect� du serveur.");
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isLocalPlayer)
        {
            ServiceLocator.Get<GameManager>().ThrowError("Vous avez �t� d�connect� du serveur.");
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

    public void TPtoHole(Transform pos)
    {
        if(isLocalPlayer)
        {
            transform.position = pos.position;
        }
    }
    [Client]
    public void TeleportToPoint(Vector3 pos)
    {
        Debug.Log("Tp de " + GetName());
        if (isLocalPlayer)
        {
            Debug.Log("Tp de " + GetName() + " vers2 " + pos);
            transform.position = pos;
            CmdTeleportToPoint(pos);
        }
        else
            CmdTeleportToPoint(pos);
    }

    [Client]
    public void TpToLocation(Transform location)
    {
        Debug.Log("tp to " + location.position);
        transform.position = location.position;
        //transform.position = new Vector3(location.position.x, location.position.y, location.position.z + id);
        SpawnBall();
    }

    public void SpawnBall()
    {
        //if (isLocalPlayer)
        {
            Debug.Log("Active  de " + GetName());
            ball.SetActive(true);
            RpcSpawnBall();
        }
    }
    [Command]
    public void CmdTeleportToPoint(Vector3 pos)
    {
        Debug.Log("Tp de " + GetName());
        //if (!isOwned)
        {
            Debug.Log("Tp de " + GetName() + " vers " + pos);
            transform.position = pos;
           // RpcTeleportToPoint(pos);
        }
    }
    [ClientRpc]
    public void RpcTeleportToPoint(Vector3 pos)
    {
        Debug.Log("Tp de " + GetName());
        //if (!isOwned)
        {
            Debug.Log("Tp de " + GetName() + " vers " + pos);
            transform.position = pos;

        }
    }
    [Command]
    public void RpcSpawnBall()
    {
        //if (isLocalPlayer)
        {
            Debug.Log("Active  de " + GetName());
            ball.SetActive(true);
        }
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
}
