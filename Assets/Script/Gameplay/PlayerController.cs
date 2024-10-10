using JetBrains.Annotations;
using Mirror;
using Mirror.Examples.Pong;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;


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
    [SerializeField] private GameObject ball;
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

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isLocalPlayer)
        {
            ServiceLocator.Get<GameManager>().ThrowError("Vous avez été déconnecté du serveur.");
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
        Destroy(playerScore);
        Destroy(gameObject);
    }

    public void TpToLocation(Transform location)
    {
        Debug.Log("tp to " + location.position);
        
        ball.GetComponent<Rigidbody>().freezeRotation = true;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
      
        ball.transform.position = transform.localPosition;
        ball.transform.rotation = Quaternion.identity;

       
        transform.position = location.position;
        ball.GetComponent<Rigidbody>().freezeRotation = false;
        //transform.position = new Vector3(location.position.x, location.position.y, location.position.z + id);
        SpawnBall();
        ball.GetComponent<BallControler>().SetLastPosition(transform.localPosition);
        ball.GetComponent<BallControler>().SetRotationValueY(location.rotation.eulerAngles.y);
    }
    public void TpToLocation(Vector3 location)
    {
        ball.GetComponent<Rigidbody>().freezeRotation = true;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.position = transform.localPosition;
        ball.transform.rotation = Quaternion.identity;
        transform.position = location;
        ball.GetComponent<Rigidbody>().freezeRotation = false;
    }
    public void SpawnBall()
    {
        ball.SetActive(true);
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
    public void PushBall(Vector3 dir,float force)
    {
        ball.GetComponent<Rigidbody>().AddForce(dir * force, ForceMode.VelocityChange);
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

    public GameObject GetBall()
    {
        return ball;
    }

    public void ActivateAll(bool b)
    {
        camObj.enabled = b;
        ball.GetComponent<BallControler>().enabled = b;
    }

    public void OnHoleEntered(int maxStrokes)
    {
        Debug.Log("Une balle est rentrée : " + playerName);
        var gm = ServiceLocator.Get<GameManager>();
        hasFinishHole = true;
        if (gm.GetLocalPlayer().netIdentity == netIdentity)
        {
            Debug.Log("play sound");
            goodHoleSound.Play();
            Debug.Log("play animation");
            StartCoroutine(CouroutineShowResultHole(strokes[actualHole], maxStrokes));
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
    private IEnumerator CouroutineShowResultHole(int actualStrokes, int maxStrokes)
    {
        yield return null;
        string result = GetTextResultHole(actualStrokes, maxStrokes);
        Debug.Log(result);
        resultHoleText.text = result;
        resultHoleText.gameObject.SetActive(true);
        resultHoleText.GetComponent<Animator>().Play("New Animation");
        yield return new WaitForSeconds(1f);
        resultHoleText.gameObject.SetActive(false);
        playerUI.NextPlayer();
    }
}
