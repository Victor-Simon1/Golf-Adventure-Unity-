using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net.Sockets;
using Services;
using System;
using UnityEngine.UI;
using System.Net.NetworkInformation;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror.Examples.Basic;

public class GameManager : MonoRegistrable
{
    [Header("List")]
    [SerializeField] private List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private List<StartBehaviour> starts = new List<StartBehaviour>();
    [SerializeField] private List<HoleBehavior> holes = new List<HoleBehavior>();
    
    [Header("Other Managers")]
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private ErrorManager em;

    [Header("Network Variables")]
    private string hostIP;
    private string sessionName;
    private bool isHost = false;

    [Header("Gameplay Variables")]
    public int actualHole = 0;
    public int nbPlayerFinishHole = 0;
    public bool inGame;
    public bool isLoaded = false;
    public bool isTpReady;

   
    [Header("Maps Variables")]
    [SerializeField] private string[] maps;
    private int mapId;

    private int nbPlayerReady;

    private void Awake()
    {
        //if(ServiceLocator.IsRegistered<GameManager>())
          //  Destroy(ServiceLocator.Get<GameManager>().gameObject);
        ServiceLocator.Register<GameManager>(this, false);
        starts.Clear();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        networkManager = ServiceLocator.Get<StockNetManager>().GetNetworkManager();
        uiManager = ServiceLocator.Get<UIManager>();
        //Display();

        
    }

    private void Update()
    {
        if (isLoaded && isTpReady)
        {
            isLoaded = false;
            var st= starts[0].transform;

            StartCoroutine(CoroutingWaitingForAllPlayers(true));

            GetLocalPlayer().CmdTpToLocation(st.position, st.rotation.eulerAngles);
        }
        if (isHost)
        {
            if (inGame)
            {
                foreach (PlayerController player in players)
                {
                    if (player != null)
                    {
                        player.RpcSimulateBall();
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        // Debug.Log(networkManager.name);
    }

    private void OnDestroy()
    {
        foreach (var player in players) { Destroy(player); }

        if(networkManager != null)
            Destroy(networkManager.gameObject);
    }

    public void ResetManager()
    {
        starts.Clear();
        holes.Clear();
        actualHole = 0;
        nbPlayerFinishHole = 0;
        StartBehaviour.max = 0;
        HoleBehavior.max = 0;
        inGame = false;
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
            //Debug.Log("Stop Server");
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
    public void RemovePc(PlayerController pc)
    {
        players.Remove(pc);
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
        try
        {
            hostIP = GetLocalIPAddress();
        }
        catch (Exception e) 
        {
            //Close the network
            networkManager.StopHost();
            ThrowError("No ipv4 address available");
            //DontDestroy.dontDestroyTargets.ForEach(e => Destroy(e));
            ServiceLocator.Get<JoinManager>().StopConnection();
            throw e;
        }
        //Fill information
        sessionName = PartyName;
        networkManager.networkAddress = hostIP;
        isHost = true;

        //Close old windows
        GameObject joinCanvas = GameObject.FindObjectsOfType<JoinManager>(true)[0].gameObject;
        joinCanvas.SetActive(false);
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
        //Debug.Log("Nombre de joueurs ayant fini "+ nbPlayerFinishHole);
        if (nbPlayerFinishHole != players.Count)
            yield return null;
        else
        {
            nbPlayerFinishHole = 0;

            //Debug.Log("Tp vers le prochain trou :" + (actualHole + 1) +" / " + starts.Count);

            if ((actualHole+1) == starts.Count)
            {
                //Debug.Log("Map fini");
                yield return new WaitForSeconds(1f);

                uiManager.GetPlayerUI().GetScoreboard().Pop(1f);

                ResetManager();

                foreach (var item in players)
                {
                    item.UpdateReady(false);
                }

                if (isHost)
                {
                    //Parent
                    GameObject gmNextMap = GameObject.Find("NextMap");
                    var changeMap = gmNextMap.transform.GetChild(0);
                    //Dropdown of map's choices
                    TMP_Dropdown dropdown = changeMap.GetChild(2).GetChild(0).gameObject.GetComponent<TMPro.TMP_Dropdown>();
                    //Button for launch a new game
                    var launchGameButton = changeMap.GetChild(2).GetChild(1).GetChild(1).GetComponent<Button>();
                    //Scoreboard of all players
                    var scoreboard = GameObject.Find("PlayerList");
                    //Setup scoreboard
                    scoreboard.transform.SetParent(changeMap);
                    scoreboard.transform.localPosition = new Vector3(0, -120, 0);
                    scoreboard.SetActive(false);
                    uiManager.GetPlayerUI().GetScoreboard().gameObject.SetActive(false);
                    changeMap.gameObject.SetActive(true);
                    
                    dropdown.onValueChanged.AddListener(
                        delegate
                        {
                            SetMapID(dropdown.value);
                        });
                   launchGameButton.onClick.AddListener(
                        delegate
                        {
                            foreach (var player in players)
                            {
                                player.RPCDisableBall();
                            }
                            LaunchGame();
                        });
                }
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
            //Debug.LogWarning("Chargement de la map");
            foreach (PlayerController player in players)
            {
                player.RpcLaunch(maps[mapId]);
            }
        }
    }

    public IEnumerator LoadScene(string mapId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(mapId);

        var loadingScreen = uiManager.GetLoadingScreen();
        var hubCanvas = uiManager.GetHubCanvas();
        if(hubCanvas != null)
            hubCanvas.SetActive(false);
        if(loadingScreen != null)
            loadingScreen.gameObject.SetActive(true);
        while (!operation.isDone)
        {
            //Debug.Log("Loading...");
            float progressiveValue = Mathf.Clamp01(operation.progress / 0.9f);
            if (loadingScreen != null)
                loadingScreen.SetProgression(progressiveValue);
            yield return null;
        }

        isLoaded = true;
    }

    public void AddStart(StartBehaviour newStart)
    {
        starts.Add(newStart);
        starts.Sort();
        //Debug.Log("Add " + starts.Count + " : "  + StartBehaviour.max);
        if (starts.Count == StartBehaviour.max)
        {
            //Debug.Log("Tp vers le premier trou");
            isTpReady = true;
        }
        else
        {
            isTpReady = false;
        }
           
    }

    public void AddHole(HoleBehavior newHole)
    {
        holes.Add(newHole);
        holes.Sort();
    }
    private void TpPlayersToLocation(int idStart = 0)
    {
        //Debug.Log("Here we go : " + idStart);
        StartCoroutine(CoroutingWaitingForAllPlayers(false));
        players.ForEach(p => {
            p.GetBall().IgnoreBalls();
            p.TpToLocation(starts[idStart].transform);
            p.hasFinishHole = false;
        });
    }

    private string GetLocalIPAddress()
    {
        foreach(UnicastIPAddressInformation add in NetworkInterface.GetAllNetworkInterfaces()[0].GetIPProperties().UnicastAddresses)
        {
            if (add.Address.AddressFamily == AddressFamily.InterNetwork)
            {
                //Debug.Log("The address :" + add.Address.ToString());
                return add.Address.ToString();
            }
        }
       throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    public void Display()
    {
        foreach (NetworkInterface netInterface in
        NetworkInterface.GetAllNetworkInterfaces())
            {
                Debug.Log("Name: " + netInterface.Name);
                Debug.Log("Description: " + netInterface.Description);
                Debug.Log("Addresses: ");

                IPInterfaceProperties ipProps = netInterface.GetIPProperties();

                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                {
                    Debug.Log(" " + addr.Address.ToString());
                }

                Debug.Log("");
            }
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

    public void SetnbReady(int n) 
    {
        nbPlayerReady = n;
    }

    public int GetnbReady()
    {
        return nbPlayerReady;
    }

    public string GetStringNbReady()
    {
        return nbPlayerReady + "/" + players.Count;
    }

    private IEnumerator CoroutingWaitingForAllPlayers(bool display)
    {
        PlayerUI playerUI = null;
        while (true)
        {
            //Debug.Log("waiting players ...");
            if(uiManager != null)
            {
                if (playerUI == null)
                {
                    playerUI = uiManager.GetPlayerUI();
                }
                else
                {
                    nbPlayerReady = 0;
                    foreach (var player in players)
                    {
                        if (player.IsReady()) nbPlayerReady++;
                    }
                    //Debug.Log("nb player ready: " + nbPlayerReady + "/" + players.Count);
                    if (nbPlayerReady == players.Count)
                    {
                        playerUI.DisplayWaiting(false, "Lancement...");
                        players.ForEach(p => p.SpawnBall());
                        break;
                    }
                    playerUI.DisplayWaiting(display, nbPlayerReady + "/" + players.Count);
                }
            }
            yield return null;
        }

        inGame = true;
    }
}
