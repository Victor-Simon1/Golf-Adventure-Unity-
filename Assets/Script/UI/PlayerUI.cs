using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Services;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI strokes;

    private void OnEnable()
    {
        var gm = ServiceLocator.Get<GameManager>();
        var pc = gm.GetLocalPlayer();
        pc.SetPlayerUI(this);
        Title.text = gm.GetSessionName();
        playerName.text = pc.GetName();
    }

    public void SetStrokes(int strokes)
    {
        this.strokes.text = "" + strokes;
    }

    public void SetName(string newName)
    {
        this.playerName.text = newName;
    }
}
