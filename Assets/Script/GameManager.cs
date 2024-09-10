using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using System.Linq;
using Mirror;
using System.Net.Sockets;
using System.Net;

public class GameManager : MonoBehaviour
{

    [SerializeField] private static List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private NetworkManager networkManager;

    private string hostIP;
    private string sessionName;

    private string currentPlayer;

    private bool isHost = false;

    private void OnEnable()
    {
        Debug.Log(networkManager.name);
    }

    public static void RegisterPlayer(int netID,PlayerController player)
    {
        players.Add(player);
    }

    public static void UnregisterPlayer(PlayerController pc) 
    {
        players.Remove(pc);
    }
    public static PlayerController GetPlayer(int playerId)
    {
        return players[playerId];
    }
    public static PlayerController[] GetAllPlayer()
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
        networkManager.networkAddress = hostIP;
        currentPlayer = connectionName;
        networkManager.StartClient();
    }

    public string GetCurrentPlayerName()
    {
        return currentPlayer;
    }
}
