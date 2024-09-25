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
using Unity.VisualScripting;

public class GameManager : MonoRegistrable
{

    [SerializeField] private List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private List<StartBehaviour> starts = new List<StartBehaviour>();
    [SerializeField] private List<HoleBehavior> holes = new List<HoleBehavior>();

    [SerializeField] private NetworkManager networkManager;

    [SerializeField] private ErrorManager em;

    private string hostIP;
    private string sessionName;
    private bool isHost = false;
    public int actualHole = 0;
    public int nbPlayerFinishHole = 0;
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

    public /*void*/IEnumerator GoNextHole()
    {
        int playerFinish = 0;
        for (int i = 0; i < players.Count; i++)
            if (players[i] != null)
                if (players[i].hasFinishHole)
                    playerFinish++;
        Debug.Log("Nombre de joueurs ayant fini"+ playerFinish);
        if (playerFinish != players.Count)
            yield return null;
        else
        {
            Debug.Log("Tp vers le prochain trou :" + (actualHole + 1));
            if (actualHole == starts.Count)
            {
                Debug.Log("Map fini");
                yield return null;
            }
            else
            {
                actualHole++;
                yield return new WaitForSeconds(1f);

                GetLocalPlayer().GetPlayerUI().GetScoreboard().Pop(1f);
                yield return new WaitForSeconds(3f) ;

                players.ForEach(p => { p.actualHole = actualHole; });
                TpPlayersToLocation(actualHole);
                yield return new WaitForSeconds(0.5f);

                GetLocalPlayer().GetPlayerUI().GetScoreboard().Pop(1f);
                yield return null;
            }
          
        }
     
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

    public void AddStart(StartBehaviour newStart)
    {
        starts.Add(newStart);
        starts.Sort();
        if (starts.Count == StartBehaviour.max) 
            TpPlayersToLocation();
    }

    public void AddHole(HoleBehavior newHole)
    {
        holes.Add(newHole);
        holes.Sort();
    }
    private void TpPlayersToLocation(int idStart = 0)
    {
        Debug.Log("Here we go : " + idStart);
        players.ForEach(p => { 
            p.TpToLocation(starts[idStart].transform);
            p.hasFinishHole = false;
        });
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
    public void SetPlayerColor(Color color, int id) 
    {
        players[id].RpcSetColor(color);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
