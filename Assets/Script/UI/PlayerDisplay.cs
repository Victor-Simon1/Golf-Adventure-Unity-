using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Services;
public class PlayerDisplay : MonoBehaviour
{

    [SerializeField] private PlayerController pc;
    [SerializeField] private Image image;

    private void Start()
    {
        if(pc.isLocalPlayer && PlayerPrefs.HasKey("CurrentHue"))
        {
            Color currentColor = Color.HSVToRGB(PlayerPrefs.GetFloat("CurrentHue"), PlayerPrefs.GetFloat("CurrentSat"), PlayerPrefs.GetFloat("CurrentVal"));
            image.color = currentColor;
            ServiceLocator.Get<GameManager>().GetLocalPlayer().SetColor(currentColor);
           
        }
    }
    private void OnEnable()
    {
        image = transform.GetComponentInChildren<Button>().transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        SetName(pc.GetName());
    }

    public void Setup(PlayerController npc)
    {
        pc = npc;
        pc.SetDisplay(this);
        gameObject.SetActive(true);
        if(npc.isLocalPlayer)
            transform.GetComponentInChildren<Button>().interactable = true;
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

    public void SetColor(Color color)
    {
        image.color = color;
    }
}
