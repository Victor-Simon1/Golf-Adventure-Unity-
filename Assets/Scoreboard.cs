using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] GameObject playerScoreboardItem;

    [SerializeField] Transform playerScoreboardList;

    private void OnEnable()
    {
        //Recupere l'array du server 
        Player[] players = GameManager.GetAllPlayer(); 
        //Loop sur l'array et ajout de ligne
        foreach (Player p in players)
        {
            Debug.Log(p.name + " " + p.strokes);
            GameObject itemGo = Instantiate(playerScoreboardItem,playerScoreboardList);
            PlayerScoreboardItem item = itemGo.GetComponent<PlayerScoreboardItem>();
            if (item)
                item.Setup(p);
        }
    }

    private void OnDisable()
    {
        foreach(Transform child in playerScoreboardList)
            Destroy(child.gameObject);
    }
}
