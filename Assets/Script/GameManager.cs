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
    [SerializeField] private List<StartColliderScript> starts = new List<StartColliderScript>();

    [SerializeField] private NetworkManager networkManager;

    [SerializeField] private ErrorManager em;

    [SerializeField] private List<HoleBehavior> holesList= new List<HoleBehavior>();

    [SerializeField] private List<StartBehavior> startList = new List<StartBehavior>();

    private string hostIP;
    private string sessionName;
    private bool isHost = false;

    public bool inGame;

    [SerializeField] private string[] maps;
    private int mapId;

    private void Awake()
    {
        ServiceLocator.Register<GameManager>(this, false);
        starts.Clear();
    }

    private void Start()
    {
        networkManager = ServiceLocator.Get<StockNetManager>().GetNetworkManager();
    }

    private void OnEnable()
    {
        // Debug.Log(networkManager.name);
    }

    private void OnDestroy()
    {
        foreach (var player in players) { Destroy(player); }
        Destroy(networkManager.gameObject);
    }

    public void RegisterPlayer(PlayerController player)
    {
        if(player.id < players.Count && players[player.id] == null)
        {
            players[player.id] = player;
        }
        else
        {
            players.Add(player);
        }

        players.Sort();
    }

    public void PlayerQuit()
    {
        if (isHost)
        {
            Debug.Log("Stop Server");
            /*foreach (PlayerController player in players)
            {
                player.CmdStopHost();
            }*/
            networkManager.StopHost();
        }
        else
        {
            foreach (PlayerController player in players)
            {
                if (player.isLocalPlayer)
                {
                    player.CmdExit();
                    break;
                }
            }
        }
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
    public List<HoleBehavior> GetListHoles()
    {
        return holesList;
    }

    public PlayerController GetLocalPlayer()
    {
        foreach(PlayerController player in players)
        {
            if(player.isLocalPlayer) return player;
        }

        return null;
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

    public void StopConnection()
    {
        networkManager.StopClient();
    }

    private void LaunchGame()
    {
        if(isHost)
        {
            foreach (PlayerController player in players)
            {
                player.RpcLaunch(maps[mapId]);
            }
        }
    }

    public void AddStart(StartColliderScript newStart)
    {
        starts.Add(newStart);
        starts.Sort();
        if (starts.Count == StartColliderScript.max) TpPlayersToLocation();
    }

    private void TpPlayersToLocation()
    {
        Debug.Log("Here we go");
        players.ForEach(p => { p.TpToLocation(starts[0].transform); });
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

    public bool IsHost()
    {
        return isHost;
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

    public void SetErrorManager(ErrorManager _em)
    {
        em = _em;
    }
    public void ThrowError(string message)
    {
        em.Error(message);
    }

    public void AddHole(HoleBehavior hb)
    {
        holesList.Add(hb);
      //  holesList.Sort();

    }
    public void AddStart(StartBehavior sb)
    {
        startList.Add(sb);
        startList.Sort();

    }
    public void TeleportToPoint(int holeId)
    {
        //if(isHost)
        {
            Debug.Log("Je commence la tp + " +players.Count );
            foreach (PlayerController p in players)
            {
                Debug.Log("Tp de " + p.GetName() + " vers " + startList[0].transform.position +"(" + startList[0] + ")");
                p.TeleportToPoint(startList[0].transform.position);//*/.transform.position = startList[0].transform.position;
                p.SpawnBall();
            }
        }
      
    }
    public void SetPlayerColor(Color color, int id) 
    {
        players[id].RpcSetColor(color);
    }

}
