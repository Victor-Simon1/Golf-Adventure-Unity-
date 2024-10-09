using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerScoreboardItem : MonoBehaviour,IComparable
{
    [SerializeField] TextMeshProUGUI usernameText;

    [SerializeField] TextMeshProUGUI strokeText;

    private int nbStrokes;
    
    public void Setup(PlayerController p)
    {
        usernameText.text = p.GetName();
        strokeText.text = "0";
        nbStrokes = 0;
        p.SetPlayerScoreboard(this);
        
    }

    public void SetSum(int sum)
    {
        nbStrokes = sum;
        strokeText.text = sum.ToString();
        nbStrokes = sum;
    }

    public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as PlayerScoreboardItem;

        if (a.nbStrokes < b.nbStrokes)
            return -1;

        if (a.nbStrokes > b.nbStrokes)
            return 1;

        return 0;
    }

    public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as PlayerScoreboardItem;
        if (a.nbStrokes < b.nbStrokes)
            return -1;
        if (a.nbStrokes > b.nbStrokes)
            return 1;
        return 0;
    }
}
