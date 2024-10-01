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

    [SerializeField] private GameObject pushButton;
    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject arrows;

    private PlayerController displayedPlayer;
    private PlayerController player;
    [SerializeField] private VictoryPopup scoreboard;

    private void OnEnable()
    {
        var gm = ServiceLocator.Get<GameManager>();
        Title.text = gm.GetSessionName();

        SetActualPlayer(gm.GetLocalPlayer());
    }

    public void SetActualPlayer(PlayerController pc)
    {
        player = pc;
        SetPlayer(player);
    }

    public void ResetPlayer()
    {
        SetPlayer(player);
    }

    public void SetPlayer(PlayerController pc)
    {
        if(displayedPlayer != null)
        {
            displayedPlayer.ActivateAll(false);
            displayedPlayer.SetPlayerUI(null);
        }
        pc.SetPlayerUI(this);
        playerName.text = pc.GetName();
        displayedPlayer = pc;
        displayedPlayer.ActivateAll(true);
    }

    public void Spectate(bool b)
    {
        pushButton.SetActive(b);
        slider.SetActive(b);
        arrows.SetActive(!b);
    }

    public void NextPlayer()
    {
        displayedPlayer.SetPlayerUI(null);
        var pl = ServiceLocator.Get<GameManager>().GetListPlayer();
        PlayerController pc = null;

        if(pl.Count == 1)
        {
            pc = pl[0];
        }
        else for (int i = pl.Count - 1; i >= 0; i--)
        {
                if (!pl[i].hasFinishHole)
                {
                    if (pl[i].id > displayedPlayer.id)
                    {
                        pc = pl[i];
                    }
                }
        }

        if(pc == null)
        {
            for (int j = 0; j < pl.Count; j++)
            {
                if (!pl[j].hasFinishHole)
                {
                    pc = pl[j];
                }
            }
        }
        SetPlayer(pc);
    }

    public void PreviousPlayer()
    {
        displayedPlayer.SetPlayerUI(null);
        var pl = ServiceLocator.Get<GameManager>().GetListPlayer();
        PlayerController pc = null;

        if(pl.Count == 1)
        {
            pc = pl[0];
        }
        else for (int i = 0; i < pl.Count; i++)
        {
                if (!pl[i].hasFinishHole)
                {
                    if (pl[i].id > displayedPlayer.id)
                    {
                        pc = pl[i];
                    }
                }
        }

        if(pc == null)
        {
            for (int j = pl.Count - 1; j >= 0; j--)
            {
                if (!pl[j].hasFinishHole)
                {
                    pc = pl[j];
                }
            }
        }
        SetPlayer(pc);
    }

    public void SetStrokes(int strokes)
    {
        this.strokes.text = "" + strokes;
    }

    public void SetName(string newName)
    {
        this.playerName.text = newName;
    }
    public VictoryPopup GetScoreboard()
    {
        return scoreboard;
    }
}
