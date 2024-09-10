using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinManager : MonoBehaviour
{
    [SerializeField] private GameManager gm;

    private string playerName;
    private string ip;

    // Start is called before the first frame update
    void Start()
    {
        playerName = "Player 1";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetName(string newName)
    {
        playerName = newName;
    }

    public void SetIp(string newIp)
    {
        ip = newIp;
    }

    public void Connect()
    {
        gm.Connection(ip, playerName);
    }
}
