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

    [Header("Material")]
    private Material mat;

    [Header("Audio")]
    [SerializeField] private AudioSource goodHoleSound;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI resultHoleText;

    [Header("Scripts")]
    private PlayerScoreboardItem playerScore;
    [SerializeField] private PlayerUI playerUI;
    private PlayerDisplay display;

    [Header("Network")]
    [SerializeField] private Behaviour nrbLocal;
    [SerializeField] private Behaviour nrbNotLocal;
    [SerializeField] private Behaviour nTransformLocal;
    [SerializeField] private Behaviour nTransformNotLocal;

    private void Start()
    {
        mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetFloat("_Glossiness", .8f);
        mat.SetFloat("_Metallic", 0f);

        ball.GetComponent<Renderer>().material = mat;

        InitStrokes();
    }

    private void Update()
    {
        if(playerUI != null)
        {
            playerUI.SetStrokes(strokes[actualHole]);
        }
        if(playerScore != null)
        {
            playerScore.SetSum(GetSumStrokes());
        }
    }

    public void Reset()
    {
        actualHole = 0;
        hasFinishHole = false;
        strokes.Clear();
        InitStrokes() ;
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isLocalPlayer)
        {
            Debug.Log("Client disconnect :" + playerName);
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
            Debug.Log("A Client has left :" + playerName);
            GameManager gm = ServiceLocator.Get<GameManager>();
            //Remove the player who quit from our manager
            gm.RemovePc(this);
            foreach(PlayerController pc in gm.GetListPlayer())
            {
                Debug.Log("name :" + pc.playerName);
            } 
        }
    }

    //[Command]
    void InitStrokes()
    {
        
        for(int i = 0; i < 18; i++)
        {
            strokes.Add(0);
        }
    }
    public int GetActualStrokes()
    {
        return strokes[actualHole];

    }
    [ClientRpc]
    public void RpcAddStroke()
    {
        strokes[actualHole]++;
    }

    [ClientRpc]
    public void RpcAddXStroke(int x)
    {
        strokes[actualHole]+=x ;
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
        Destroy(playerScore);
        Destroy(gameObject);
    }

    public void TpToLocation(Transform location)
    {
        Debug.Log("tp to " + location.position);

        var ballRb = ball.GetComponent<Rigidbody>();
        ballRb.freezeRotation = true;
        ballRb.velocity = Vector3.zero;
      
        ball.transform.position = transform.localPosition;
        ball.transform.rotation = Quaternion.identity;

       
        transform.position = location.position;
        ballRb.freezeRotation = false;
        //transform.position = new Vector3(location.position.x, location.position.y, location.position.z + id);
        ball.SetLastPosition(transform.localPosition);
        ball.SetRotationValueY(location.rotation.eulerAngles.y);

        ballRb.useGravity = true;
    }
    public void TpToLocation(Vector3 location)
    {
        //Debug.Log("Deubt tp to location:" + ball.transform.position);
        ball.GetComponent<Rigidbody>().freezeRotation = true;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.position = location;//transform.localPosition;
        ball.transform.rotation = Quaternion.identity;
        //transform.position = location;
        ball.GetComponent<Rigidbody>().freezeRotation = false;
        //Debug.Log("Fin tp to location" + ball.transform.position);
    }

    public void SpawnBall()
    {
        ball.enabled = true;
        ball.Spawn(true);
    }

    public void DespawnBall()
    {
        ball.enabled = true;
        ball.Spawn(false);
    }

    [ClientRpc]
    public void RpcSpawnBalls()
    {
        if(isLocalPlayer) ServiceLocator.Get<GameManager>().GetListPlayer().ForEach(p => p.SpawnBall());
    }

    public void SetColor(Color color)
    {
        CmdSetColor(color, id);
    }
    [Command]
    public void PushBall(Vector3 dir,float force)
    {
        ball.GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.Impulse);
        ball.GetComponent<BallControler>().moving = true;
        RpcAddStroke();
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
        mat.SetColor("_BaseColor", color);
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

    public int GetSumStrokes()
    {
        int sum = 0;
        strokes.ForEach(s => sum += s) ;
        return sum;
    }

    public BallControler GetBall()
    {
        return ball;
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
        DespawnBall();
        if (gm.GetLocalPlayer().netIdentity == netIdentity)
        {
#if UNITY_ANDROID //&& !UNITY_EDITOR
            Debug.Log("Je vibre");
            Handheld.Vibrate();
#endif
            Debug.Log("play sound");
            goodHoleSound.Play();
            Debug.Log("play animation");
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
        if (gm.GetLocalPlayer().netIdentity == netIdentity)
        {
            Debug.Log("play sound");
            //goodHoleSound.Play();
            StartCoroutine(CouroutineShowResultHole(strokes[actualHole], 0,true));
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
            Debug.Log("play sound");
            //goodHoleSound.Play();
            StartCoroutine(CouroutineShowResultHole(strokes[actualHole], 0, false,true));
            playerUI.Spectate(true);
        }
        gm.PlayerFinished();
        StartCoroutine(gm.GoNextHole());
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
    private IEnumerator CouroutineShowResultHole(int actualStrokes, int maxStrokes,bool outOfStrokes = false, bool outOfTime = false)
    {
        yield return null;
        string result = GetTextResultHole(actualStrokes, maxStrokes);
        if (outOfStrokes)
            result = "OUT OF STROKES";
        if (outOfTime)
            result = "OUT OF TIME";
        Debug.Log(result);
        resultHoleText.text = result;
        resultHoleText.gameObject.SetActive(true);
        resultHoleText.GetComponent<Animator>().Play("New Animation");
        yield return new WaitForSeconds(1f);
        resultHoleText.gameObject.SetActive(false);
        playerUI.NextPlayer();
    }

    public void hasArrived()
    {
        CmdAskGMReady();
    }

    [ClientRpc]
    public void DisplayNbReady(bool b, int nbReady)
    {
        ServiceLocator.Get<GameManager>().SetnbReady(nbReady);
        playerUI.DisplayWaiting(b);
    }


    [Command]
    public void CmdAskGMReady()
    {
        ServiceLocator.Get<GameManager>().addReady();
    }
}
