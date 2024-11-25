using System;
using TMPro;
using UnityEngine;

public class PlayerScoreboardItem : MonoBehaviour,IComparable
{
    [Header("Gameobject")]
    [SerializeField] TextMeshProUGUI usernameText;
    [SerializeField] TextMeshProUGUI strokeText;
    [Header("Variables")]
    private int nbStrokes;

#region PUBLIC_FUNCTION
    public void Setup(PlayerController p)
    {
        usernameText.text = p.GetName();
        strokeText.text = "0";
        nbStrokes = 0;
        p.SetPlayerScoreboard(this);
        
    }
    public void SetSum(int sum)
    {
        strokeText.text = sum.ToString();
        nbStrokes = sum;
    }

    public void SetupAndSum(PlayerController p, int sum)
    {
        usernameText.text = p.GetName();
        strokeText.text = sum.ToString();
        nbStrokes = sum;
    }
    #endregion

#region ICOMPARABLE_FUNCTION
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
#endregion
}
