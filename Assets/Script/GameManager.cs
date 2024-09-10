using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using System.Linq;
using Mirror;
using System.Net.Sockets;
using System.Net;
using Services;

public class GameManager : MonoRegistrable
{

    [SerializeField] private static List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private NetworkManager networkManager;

    private string hostIP;
    private string sessionName;

    private string currentPlayer;

    private bool isHost = false;

    private void Start()
    {
        ServiceLocator.Register<GameManager>(this);
    }

    private void OnEnable()
    {
        Debug.Log(networkManager.name);
    }

    public void RegisterPlayer(int netID,PlayerController player)
    {
        players.Add(player);
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

    public void CreateParty()
    {
        networkManager.StartHost();
        hostIP = GetLocalIPAddress();
        sessionName = hostIP;
        networkManager.networkAddress = hostIP;
        isHost = true;
    }

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

    public void Connection(string connectionIp, string connectionName)
    {
        hostIP = connectionIp;
        Debug.Log(hostIP);
        networkManager.networkAddress = hostIP;
        Debug.Log(networkManager.networkAddress);
        currentPlayer = connectionName;
        networkManager.StartClient();
    }

    public string GetCurrentPlayerName()
    {
        return currentPlayer;
    }
}
