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
using UnityEngine.UI;

public class GameManager : MonoRegistrable
{

    [SerializeField] private List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private List<StartBehaviour> starts = new List<StartBehaviour>();
    [SerializeField] private List<HoleBehavior> holes = new List<HoleBehavior>();

    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private UIManager uiManager;

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

    public void ResetManager()
    {
        starts.Clear();
        holes.Clear();
        actualHole = 0;
        nbPlayerFinishHole = 0;
        StartBehaviour.max = 0;
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

    public void PlayerFinished()
    {
        nbPlayerFinishHole++;
    }

    public IEnumerator GoNextHole()
    {
        Debug.Log("Nombre de joueurs ayant fini "+ nbPlayerFinishHole);
        if (nbPlayerFinishHole != players.Count)
            yield return null;
        else
        {
            nbPlayerFinishHole = 0;

            Debug.Log("Tp vers le prochain trou :" + (actualHole + 1) +" / " + starts.Count);
            if ((actualHole+1) == starts.Count)
            {
                Debug.Log("Map fini");
                yield return new WaitForSeconds(1f);

                uiManager.GetPlayerUI().GetScoreboard().Pop(1f);
                
                if(isHost)
                {
                    GameObject gmNextMap = GameObject.Find("NextMap");
                    gmNextMap.transform.GetChild(0).gameObject.SetActive(true);
                    TMPro.TMP_Dropdown dropdown = gmNextMap.transform.GetChild(0).gameObject.GetComponent<TMPro.TMP_Dropdown>();
                    dropdown.onValueChanged.AddListener(
                        delegate
                        {
                            SetMapID(dropdown.value);
                        });
                    gmNextMap.transform.GetChild(1).gameObject.SetActive(true);
                    gmNextMap.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(
                        delegate
                        {
                            LaunchGame();
                            foreach (var player in players)
                            {
                                TpPlayersToLocation(0);
                                player.SpawnBall();
                            }

                        });
                }
                foreach(var player in players)
                    player.DespawnBall();
                ResetManager();
            }
            else
            {
                actualHole++;
                yield return new WaitForSeconds(1f);
                uiManager.GetPlayerUI().GetScoreboard().Pop(1f);
                yield return new WaitForSeconds(3f) ;

                players.ForEach(p => { p.actualHole = actualHole; });
                uiManager.GetPlayerUI().ResetAllUI();
                TpPlayersToLocation(actualHole);
                yield return new WaitForSeconds(0.5f);

                uiManager.GetPlayerUI().GetScoreboard().Pop(1f);
                yield return null;
            }
          
        }
     
    }
    private void LaunchGame()
    {
        if(isHost)
        {
            Debug.LogWarning("Chargement de la map");
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
        Debug.Log("Add " + starts.Count + " : "  + StartBehaviour.max);
        if (starts.Count == StartBehaviour.max)
        {
            Debug.Log("Tp vers le premier trou");
            TpPlayersToLocation();
        }
           
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
            p.GetBall().GetComponent<BallControler>().IgnoreBalls();
            p.TpToLocation(starts[idStart].transform);
            p.GetBall().gameObject.SetActive(true);
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

    public void setUIManager(UIManager uiManager)
    {
        this.uiManager = uiManager;
    }

}
