using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;
using System.Linq;
using Mirror;
using System.Net.Sockets;
using System.Net;
using Services;
using System;

public class GameManager : MonoRegistrable
{

    [SerializeField] private List<PlayerController> players = new List<PlayerController>();

    [SerializeField] private NetworkManager networkManager;

    private string hostIP;
    private string sessionName;

    private bool isHost = false;

    [SerializeField] private string[] maps;
    private int mapId;

    private void Awake()
    {
        ServiceLocator.Register<GameManager>(this);
    }

    private void OnEnable()
    {
       // Debug.Log(networkManager.name);
    }

    public void RegisterPlayer(PlayerController player)
    {
        players.Add(player);
        players.Sort();
    }

    public void UnregisterPlayer(PlayerController pc) 
    {
        players.Remove(pc);
    }
    public PlayerController GetPlayer(int playerId)
    {
        return players[playerId];
    }
    public PlayerController[] GetAllPlayer()
    {
        return players.ToArray();
    }
    public List<PlayerController> GetListPlayer()
    {
        return players;
    }

    public void CreateParty(string PartyName)
    {
        networkManager.StartHost();
        hostIP = GetLocalIPAddress();
        sessionName = PartyName;
        networkManager.networkAddress = hostIP;
        isHost = true;
    }

    public void Connection(string connectionIp)
    {
        hostIP = connectionIp;
        networkManager.networkAddress = hostIP;
        networkManager.StartClient();
    }

    /*[ClientRpc]
    private void RpcLaunchGame(int mapId)
    {
        Debug.Log("Change scene");
        SceneManager.LoadScene(maps[mapId]);
    }*/

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    public string GetIP()
    {
        return hostIP;
    }

    public string GetSessionName()
    {
        return sessionName;
    }

    public void SetSessionName(string newName)
    {
        sessionName = newName;
    }

    public void SetMapID(int mapID)
    {
        mapId = mapID;
    }

    public int GetMapID()
    {
        return mapId;
    }

}
