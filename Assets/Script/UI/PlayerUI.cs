using UnityEngine;
using TMPro;
using Services;

public class PlayerUI : MonoBehaviour
{
    [Header("Gameobjects")]
    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI strokes;

    [SerializeField] private GameObject pushButton;
    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject arrows;
    [Header("Script")]
    private PlayerController displayedPlayer;
    private PlayerController player;
    [SerializeField] private VictoryPopup scoreboard;

    [SerializeField] private GameObject WaitingScreen;
    [SerializeField] private TextMeshProUGUI nbPlayerWaited;

    private GameManager gm;

#region UNITY_FUNCTION
    private void OnEnable()
    {
        gm = ServiceLocator.Get<GameManager>();
        Title.text = gm.GetSessionName();

        SetActualPlayer(gm.GetLocalPlayer());

        ServiceLocator.Get<UIManager>().SetPlayerUI(this);
    }
    #endregion

#region PUBLIC_FUNCTION
    public void Spectate(bool b)
    {
        pushButton.SetActive(!b);
        slider.SetActive(!b);
        arrows.SetActive(b);
    }

    public void SetActualPlayer(PlayerController pc)
    {
        pc.SetResultHoleText(transform.GetChild(0).Find("ResultHole").GetChild(0).GetComponent<TextMeshProUGUI>());

        player = pc;
        SetPlayer(player);
    }

    public void SetPlayer(PlayerController pc)
    {
        if(pc != null)
        {
            if (displayedPlayer != null)
            {
                displayedPlayer.ActivateAll(false);
                displayedPlayer.SetPlayerUI(null);
            }
            Debug.Log("UI updated to player: " + pc.GetName());
            pc.SetPlayerUI(this);
            playerName.text = pc.GetName();
            strokes.text = pc.GetActualStrokes().ToString();
            displayedPlayer = pc;
            displayedPlayer.ActivateAll(true);
        }
    }

    public void ResetPlayer()
    {
        SetPlayer(player);
    }

    public void NextPlayer()
    {
        var gm = ServiceLocator.Get<GameManager>();
        if (gm.GetListPlayer().Count > 1)
        {
            displayedPlayer.SetPlayerUI(null);
            var pl = gm.GetListPlayer();
            PlayerController pc = null;

            if (pl.Count == 1)
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

            if (pc == null)
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

    public void ResetAllUI()
    {
        ResetPlayer();
        Spectate(false);
    }
#endregion

#region GETTER_SETTER
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

    public void DisplayWaiting(bool b, string nbPlayerReady)
    {
        WaitingScreen.SetActive(b);
        nbPlayerWaited.text = nbPlayerReady;
    }
#endregion
}
