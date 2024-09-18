using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerScoreboardItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI usernameText;

    [SerializeField] TextMeshProUGUI strokeText;


    public void Setup(PlayerController p)
    {
        usernameText.text = p.GetName();
        strokeText.text = "0";
        p.SetPlayerScoreboard(this);
        
    }

    public void SetSum(int sum)
    {
        strokeText.text = sum.ToString();
    }
}
