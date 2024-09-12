using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerListScript : MonoBehaviour
{
    [SerializeField] private GameObject PlayerTemplate;

    public void AddPlayer(PlayerController pc)
    {
        PlayerDisplay pl = Instantiate(PlayerTemplate, transform).AddComponent<PlayerDisplay>();
        pl.Setup(pc);
    }
    
    public void UpdatePlayers(List<PlayerController> pcs)
    {

        foreach(Transform child in transform)
        {
            if(child.gameObject != PlayerTemplate) Destroy(child.gameObject);
        }
        //players.ForEach(p => Destroy(p));
        pcs.ForEach(pc => AddPlayer(pc));
    }
}
