using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Services;

public class JoinManager : MonoRegistrable
{
    [SerializeField] private GameManager gm;

    private string playerName;
    private string sessionName;
    private string ip;

    // Start is called before the first frame update
    void Start()
    {
        playerName = "Player 1";
        ServiceLocator.Register<JoinManager>(this);
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
        gm.Connection(ip);
    }

    public void Create()
    {
        gm.CreateParty(sessionName);
    }

    public string GetPlayerName()
    {
        return playerName;
    }
}
