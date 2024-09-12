using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Services;

public class JoinManager : MonoRegistrable
{
    [SerializeField] private GameManager gm;
    [SerializeField] private GameObject waiting;
    [SerializeField] private GameObject next;

    private string playerName;
    private string sessionName;
    private string ip;

    // Start is called before the first frame update
    void Start()
    {
        playerName = "Player 1";
        ServiceLocator.Register<JoinManager>(this, false);
        gm = ServiceLocator.Get<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetName(string newName)
    {
        playerName = newName;
    }

    public void SetSessionName(string sessionName)
    {
        this.sessionName = sessionName;
    }

    public void SetIp(string newIp)
    {
        ip = newIp;
    }

    public void Connect()
    {
        waiting.SetActive(true);
        gm.Connection(ip);
    }

    public void Create()
    {
        gm.CreateParty(sessionName);
    }

    public void StopConnection()
    {
        waiting.SetActive(false);
        gm.StopConnection();
    }

    public void Connected()
    {
        waiting.SetActive(false);
        gameObject.SetActive(false);
        next.SetActive(true);
    }

    public string GetPlayerName()
    {
        return playerName;
    }
}
