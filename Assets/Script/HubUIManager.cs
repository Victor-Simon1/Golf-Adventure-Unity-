using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HubUIManager : MonoBehaviour
{
    [SerializeField] private GameManager gm;
    [SerializeField] private TMP_InputField sessionName;
    [SerializeField] private TextMeshProUGUI ip;

    private void OnEnable()
    {
        ip.text = "IP: " + gm.GetIP();
        sessionName.SetTextWithoutNotify(gm.GetSessionName());
    }
}
