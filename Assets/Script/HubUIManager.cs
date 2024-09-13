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
    [SerializeField] private PlayerListScript pl;

    [SerializeField] private GameObject launchButton;
    [SerializeField] private GameObject TextDropDown;
    [SerializeField] private TMP_Dropdown dropdown;

    private void Start()
    {
        ServiceLocator.Register<HubUIManager>(this, false);
    }

    private void OnEnable()
    {
        ip.text = "IP: " + gm.GetIP();
        if(ServiceLocator.Get<GameManager>().IsHost()) 
        {
            dropdown.interactable = true;
            launchButton.SetActive(true);
            TextDropDown.SetActive(true);
        }
    }

    public void SetSessionNameWithoutNotify(string sessionName)
    {
        Debug.Log("change de titre");
        this.sessionName.SetTextWithoutNotify(sessionName);
    }

    public void UpdatePlayers(List<PlayerController> pcs)
    {
        pl.UpdatePlayers(pcs);
    }
}
