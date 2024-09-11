using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Services;

public class HubUIManager : MonoRegistrable
{
    [SerializeField] private GameManager gm;
    [SerializeField] private TMP_InputField sessionName;
    [SerializeField] private TextMeshProUGUI ip;

    private void Start()
    {
        ServiceLocator.Register<HubUIManager>(this);
    }

    private void OnEnable()
    {
        ip.text = "IP: " + gm.GetIP();
        //sessionName.SetTextWithoutNotify(gm.GetSessionName());
    }

    public void SetSessionNameWithoutNotify(string sessionName)
    {
        Debug.Log("change de titre");
        this.sessionName.SetTextWithoutNotify(sessionName);
    }
}
