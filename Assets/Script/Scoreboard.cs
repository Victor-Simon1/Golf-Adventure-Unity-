using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] GameObject playerScoreboardItem;

    [SerializeField] Transform playerScoreboardList;

    private void Awake()
    {
        //Recupere l'array du server 
        List<PlayerController> players = ServiceLocator.Get<GameManager>().GetListPlayer(); 
        //Loop sur l'array et ajout de ligne
        foreach (PlayerController p in players)
        {
            GameObject itemGo = Instantiate(playerScoreboardItem,playerScoreboardList);
            itemGo.SetActive(true);
            PlayerScoreboardItem item = itemGo.GetComponent<PlayerScoreboardItem>();
            if (item != null)
                item.Setup(p);
        }
    }
}
