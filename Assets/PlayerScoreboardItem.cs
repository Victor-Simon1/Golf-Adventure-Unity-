using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerScoreboardItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI usernameText;

    [SerializeField] TextMeshProUGUI strokeText;


    public void Setup(Player p)
    {
        usernameText.text = p.name;
        strokeText.text = p.strokes.ToString();

    }
}
