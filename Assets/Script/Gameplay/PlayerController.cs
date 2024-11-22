using Mirror;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour, IComparable
{
    [Header("Holes")]
    [SerializeField] public List<int> strokes = new List<int>();
    [SyncVar] public int actualHole;
    public bool hasFinishHole = false;

    [Header("Player info")]
    [SyncVar] [SerializeField] private string playerName = "Player";
    public int id;

    [Header("Gameobjects")]
    [SerializeField] private BallControler ball;
    [SerializeField] private Camera camObj;
    [SerializeField] private Rigidbody ballRb;
    [Header("Material")]
    private Material mat;

    [Header("Audio")]
    [SerializeField] private AudioSource goodHoleSound;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI resultHoleText;
    private Projection projector;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Scripts")]
    [SerializeField] private PlayerScoreboardItem playerScore;
    [SerializeField] private PlayerUI playerUI;
    private PlayerDisplay display;
    [SerializeField] Timer timer;
    [Header("Network")]
    [SerializeField] private Behaviour nrbLocal;
    [SerializeField] private Behaviour nrbNotLocal;
    [SerializeField] private Behaviour nTransformLocal;
    [SerializeField] private Behaviour nTransformNotLocal;
    [SyncVar]
    private bool isReady;
    private bool isSimulating = false;

    #region UNITY_FUNCTION
    private void Start()
    {
        mat = ball.GetComponent<Renderer>().material;
        mat.SetFloat("_Glossiness", .8f);
        mat.SetFloat("_Metallic", 0f);

        ballRb = ball.GetComponent<Rigidbody>();
        InitStrokes();
    }

    public void Reset()
    {
        actualHole = 0;
        hasFinishHole = false;
        strokes.Clear();
        InitStrokes();
    }
    #endregion

    #region NETWORK_FUNCTION
    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isLocalPlayer)
        {
            //Debug.Log("Client disconnect :" + playerName);
            Destroy(GameObject.Find("SFXAudioSource"));
            Destroy(ServiceLocator.Get<StockNetManager>().gameObject);
            //Reset static variables
            StartBehaviour.max = 0;
            HoleBehavior.max = 0;
            //Load Hub scene
            SceneManager.LoadScene("MenuPrincipal");
            //ServiceLocator.Get<GameManager>().ThrowError("Vous avez été déconnecté du serveur.");
        }
        else
        {
            //Debug.Log("A Client has left :" + playerName);
            GameManager gm = ServiceLocator.Get<GameManager>();
            //Remove the player who quit from our manager
            gm.RemovePc(this);
            //foreach(PlayerController pc in gm.GetListPlayer())
            //{
              //  Debug.Log("name :" + pc.playerName);
            //} 
        }
    }
    #endregion

    #region RPC_FUNCTION
    [ClientRpc]
    public void RpcAddStroke()
    {
        strokes[actualHole]++;
        if(isLocalPlayer)
        {
            if (playerUI != null)
                playerUI.SetStrokes(strokes[actualHole]);
            if (playerScore != null)
                playerScore.SetSum(GetSumStrokes());
        }
    }

    [ClientRpc]
    public void RpcAddXStroke(int x)
    {
        strokes[actualHole] += x;
        if (isLocalPlayer)
        {
            if (playerUI != null)
                playerUI.SetStrokes(strokes[actualHole]);
            if (playerScore != null)
                playerScore.SetSum(GetSumStrokes());
        }
    }

    [ClientRpc]
    public void RpcFinishFirstPut()
    {
        GetComponent<SphereCollider>().excludeLayers = 0;
    }

    [ClientRpc]
    public void RpcExit()
    {
        var gm = ServiceLocator.Get<GameManager>();
        gm.StopConnection();
        PlayerDestroy();
    }

    [ClientRpc]
    public void RpcStopHost()
    {
        ServiceLocator.Get<GameManager>().ThrowError("Vous avez été déconnecté du serveur.");
    }

    [ClientRpc]
    public void RpcSetColor(Color color)
    {
        display.SetColor(color);
        mat.SetColor("_BaseColor", color);
    }
    //Call when we start a new map(sort of Reset)
    [ClientRpc]
    public void RPCDisableBall()
    {
        if (isLocalPlayer)
        {
            ball.GetComponent<BallControler>().enabled = false;
            ball.SetFirstEnable(true);
            for (int index = 0; index < strokes.Count; index++)
                strokes[index] = 0;
        }
    }
    [ClientRpc]
    public void RpcLaunch(string mapId)
    {
        if (isLocalPlayer)
        {
            var gm = ServiceLocator.Get<GameManager>();
            StartCoroutine(gm.LoadScene(mapId));
        }
    }
    [ClientRpc]
    public void RpcSpawnBalls()
    {
        if (isLocalPlayer) ServiceLocator.Get<GameManager>().GetListPlayer().ForEach(p => p.SpawnBall());
    }

    [ClientRpc]
    public void DisplayNbReady(int b, int nbReady)
    {
        ServiceLocator.Get<GameManager>().SetnbReady(nbReady);
    }

    [ClientRpc]
    public void RpcSimulateBall()
    {
        if (ServiceLocator.Get<GameManager>().inGame && isLocalPlayer && isSimulating)
        {
            projector.SimulateTrajectory(ball.transform.position, ball.GetDir(), 25);
        }
    }

    #endregion
    
    #region COMMAND_FUNCTION

    [Command]
    public void CmdExit()
    {
        RpcExit();
        PlayerDestroy();
    }

    [Command]
    public void CmdStopHost()
    {
        RpcStopHost();
    }

    [Command]
    public void PushBall(Vector3 dir, float force)
    {
        ballRb.AddForce(dir * force, ForceMode.Impulse);
        ball.GetComponent<BallControler>().moving = true;
        RpcAddStroke();
    }

    [Command]
    public void CmdSetColor(Color color, int id)
    {
        ServiceLocator.Get<GameManager>().SetPlayerColor(color, id);
    }

    [Command]
    public void UpdateReady(bool b)
    {
        isReady = b;
    }

    [Command]
    public void CmdTpToLocation(Vector3 Lposition, Vector3 Langles)
    {
        ball.UnfreezeBall();
        //Debug.Log("tp to " + location.position);
        ballRb.freezeRotation = true;
        ballRb.velocity = Vector3.zero;

        ball.IgnoreBalls();

        ball.transform.position = transform.localPosition;
        ball.transform.rotation = Quaternion.identity;

        transform.position = Lposition;
        ballRb.freezeRotation = false;
        //transform.position = new Vector3(location.position.x, location.position.y, location.position.z + id);
        ball.SetLastPosition(transform.localPosition);
        ball.SetRotationValueY(Langles.y);
        Physics.SyncTransforms();
        ball.SetAddPenality(false);
        /* (timer)
            timer.StartTimer();
        else
            Debug.LogError("Timer not init");*/
        // ball.timeLimitCoroutine = ball.StartCoroutine(ball.TimeLimit());

        RpcDoSimulate();
    }
    #endregion

    #region PUBLIC_FUNCTION
    public void hasArrived()
    {
        //Debug.Log(playerName + " has arrived");
        UpdateReady(true);
    }

    public int GetSumStrokes()
    {
        int sum = 0;
        strokes.ForEach(s => sum += s);
        return sum;
    }


    public void ActivateAll(bool b)
    {
        camObj.enabled = b;
        ball.SetIsVisualized(b);
    }

    public void OnHoleEntered(int maxStrokes)
    {
        Debug.Log("Une balle est rentrée : " + playerName);
        var gm = ServiceLocator.Get<GameManager>();
        hasFinishHole = true;
        isSimulating = false;
        ball.FreezeBall();
        DespawnBall();

        if (gm.GetLocalPlayer().netIdentity == netIdentity)
        {
#if UNITY_ANDROID //&& !UNITY_EDITOR
            //Debug.Log("Je vibre");
            Handheld.Vibrate();
#endif
            goodHoleSound.Play();
            StartCoroutine(CouroutineShowResultHole(strokes[actualHole], maxStrokes));
            playerUI.Spectate(true);
        }

        gm.PlayerFinished();
        StartCoroutine(gm.GoNextHole());

    }

    public void OnOutofStrokes()
    {
        var gm = ServiceLocator.Get<GameManager>();
        hasFinishHole = true;
        DespawnBall();
        if (gm.GetLocalPlayer().netIdentity == netIdentity)
        {
            StartCoroutine(CouroutineShowResultHole(strokes[actualHole], 0, true));
            playerUI.Spectate(true);
        }
        gm.PlayerFinished();
        StartCoroutine(gm.GoNextHole());
    }

    public void OutOfTime()
    {
        var gm = ServiceLocator.Get<GameManager>();
        hasFinishHole = true;
        if (gm.GetLocalPlayer().netIdentity == netIdentity)
        {
            StartCoroutine(CouroutineShowResultHole(strokes[actualHole], 0, false, true));
            playerUI.Spectate(true);
        }
        gm.PlayerFinished();
        StartCoroutine(gm.GoNextHole());
    }
    public int GetActualStrokes()
    {
        return strokes[actualHole];
    }

    public void TpToLocation(Transform location)
    {
        ball.UnfreezeBall();
        //Debug.Log("tp to " + location.position);
        ballRb.freezeRotation = true;
        ballRb.velocity = Vector3.zero;

        ball.transform.position = transform.localPosition;
        ball.transform.rotation = Quaternion.identity;

        transform.position = location.position;
        ballRb.freezeRotation = false;
        //transform.position = new Vector3(location.position.x, location.position.y, location.position.z + id);
        ball.SetLastPosition(transform.localPosition);
        ball.SetRotationValueY(location.rotation.eulerAngles.y);
        Physics.SyncTransforms();
        ball.SetAddPenality(false);
       /*f (timer)
            timer.StartTimer();
        else
            Debug.LogError("Timer not init");*/
       // ball.timeLimitCoroutine = ball.StartCoroutine(ball.TimeLimit());

        isSimulating = true;
    }
    public void TpToLocation(Vector3 location)
    {
        ball.UnfreezeBall();
        //Debug.Log("Deubt tp to location:" + ball.transform.position);
        ballRb.freezeRotation = true;
        ballRb.velocity = Vector3.zero;
        ball.transform.position = location;//transform.localPosition;
        ball.transform.rotation = Quaternion.identity;
        //transform.position = location;
        ballRb.freezeRotation = false;
        //Debug.Log("Fin tp to location" + ball.transform.position);
        Physics.SyncTransforms();
        isSimulating = true;
    }

    public void SpawnBall()
    {
        ball.enabled = true;
        ball.Spawn(true);

        projector = ServiceLocator.Get<Projection>();
    }

    public void DespawnBall()
    {
        ball.enabled = true;
        ball.Spawn(false);

        if (isLocalPlayer) UpdateReady(false);
    }

    public void SetColor(Color color)
    {
        CmdSetColor(color, id);
    }
    #endregion

    #region PRIVATE_FUNCTION
    void InitStrokes()
    {
        for(int i = 0; i < 18; i++)
        {
            strokes.Add(0);
        }
    }
    private void PlayerDestroy()
    {
        Destroy(display.gameObject);
        Destroy(playerScore);
        Destroy(gameObject);
    }

    private string GetTextResultHole(int actualStrokes, int maxStrokes)
    {
        int result = actualStrokes - maxStrokes;

        if (actualStrokes == 1)
            return "HOLE IN ONE";
        else if (result == -4)
            return "CONDOR";
        else if (result == -3)
            return "ALBATROS";
        else if (result == -2)
            return "EAGLE";
        else if (result == -1)
            return "BIRDIE";
        else if (result == 0)
            return "PAR";
        else if (result == 1)
            return "BOGEY";
        else if (result == 2)
            return "DOUBLE BOGEY";
        else
            return "X BOGEY";
    }


    #endregion

    #region COROUTINE
    private IEnumerator CouroutineShowResultHole(int actualStrokes, int maxStrokes, bool outOfStrokes = false, bool outOfTime = false)
    {
        yield return null;
        string result = GetTextResultHole(actualStrokes, maxStrokes);
        if (outOfStrokes)
            result = "OUT OF STROKES";
        else if (outOfTime)
            result = "OUT OF TIME";

        resultHoleText.text = result;
        resultHoleText.gameObject.SetActive(true);
        resultHoleText.GetComponent<Animator>().Play("New Animation");
        yield return new WaitForSeconds(1f);
        resultHoleText.gameObject.SetActive(false);
        playerUI.NextPlayer();
    }
    #endregion

    #region GETTER_SETTER
    public Timer GetTimer()
    {
        return timer; 
    }
    public void SetTimer(Timer _timer)
    {
        timer = _timer;
    }
    public void SetName(string newName)
    {
        this.playerName = newName;
    }

    public string GetName()
    {
        return playerName;
    }

    public void SetDisplay(PlayerDisplay display)
    {
        this.display = display;
    }
    public PlayerUI GetPlayerUI()
    {
        return playerUI;
    }
    public void SetPlayerUI(PlayerUI playerUI)
    {
        this.playerUI = playerUI;
    }

    public void SetResultHoleText(TextMeshProUGUI textMeshProUGUI)
    {
        resultHoleText = textMeshProUGUI;
    }

    public void SetPlayerScoreboard(PlayerScoreboardItem playerScore)
    {
        this.playerScore = playerScore;
    }
    public BallControler GetBall()
    {
        return ball;
    }
    public bool IsReady()
    {
        return isReady;
    }

    public LineRenderer GetLineRenderer()
    {
        return lineRenderer;
    }

    public void NotSimulate()
    {
        lineRenderer.enabled = false;
        isSimulating = false;
    }

    [ClientRpc]
    public void RpcDoSimulate()
    {
        if (!hasFinishHole)
        {
            lineRenderer.enabled = true;
            isSimulating = true;
        }
    }
    #endregion


    #region ICOMPARABLE_FUNCTION
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

    #endregion

}
