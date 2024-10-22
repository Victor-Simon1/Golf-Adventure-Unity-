using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Services;
public class PlayerDisplay : MonoBehaviour
{
    [Header("Script")]
    [SerializeField] private PlayerController pc;
    [Header("Gameobject")]
    [SerializeField] private Image image;

#region UNITY_FUNCTION
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
    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        SetName(pc.GetName());
    }

#endregion

#region PUBLIC_FUNCTION
    public void Setup(PlayerController npc)
    {
        pc = npc;
        pc.SetDisplay(this);
        gameObject.SetActive(true);
        if(npc.isLocalPlayer)
            transform.GetComponentInChildren<Button>().interactable = true;
    }
#endregion

#region GETTER_SETTER
    public void SetName(string newName)
    {
        name = pc.GetName() + "Display";
        transform.GetComponentInChildren<TextMeshProUGUI>().text = pc.GetName();
    }


    public void SetColor(Color color)
    {
        image.color = color;
    }
#endregion
}
