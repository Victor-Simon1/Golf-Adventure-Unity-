using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{

    [SerializeField] private PlayerController pc;

    private void Update()
    {
        SetName(pc.GetName());
    }

    public void Setup(PlayerController npc)
    {
        pc = npc;
        pc.SetDisplay(this);
        gameObject.SetActive(true);
    }

    public void SetName(string newName)
    {
        name = pc.GetName() + "Display";
        transform.GetComponentInChildren<TextMeshProUGUI>().text = pc.GetName();
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}
