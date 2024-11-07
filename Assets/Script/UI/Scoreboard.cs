using System.Collections.Generic;
using UnityEngine;
using Services;

public class Scoreboard : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] GameObject playerScoreboardItem;
    [SerializeField] Transform playerScoreboardList;

    [SerializeField] List<PlayerScoreboardItem> ScoreboardItems = new List<PlayerScoreboardItem>();


#region UNITY_FUNCTION
    private void Start()
    {
        Init();
        gameObject.SetActive(false);
    }
#endregion

#region PRIVATE_FUNCTION
    private void Init()
    {
        //Recupere l'array du server 
        List<PlayerController> players = ServiceLocator.Get<GameManager>().GetListPlayer();
        //Loop sur l'array et ajout de ligne
        foreach (PlayerController p in players)
        {
            GameObject itemGo = Instantiate(playerScoreboardItem, playerScoreboardList);
            itemGo.SetActive(true);
            PlayerScoreboardItem item = itemGo.GetComponent<PlayerScoreboardItem>();
            ScoreboardItems.Add(item);
            if (item != null)
                item.Setup(p);
        }
    }
#endregion

#region PUBLIC_FUNCTION
    public void TriPlayer()
    {
        ScoreboardItems.Sort();
        int i = 1;
        foreach (PlayerScoreboardItem item in ScoreboardItems)
        {
            item.transform.SetSiblingIndex(i++);
        }
    }
#endregion
}
