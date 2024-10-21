using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Services;

public class ErrorManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button quitter;

    private void Start()
    {
        quitter.onClick.AddListener(Quitter);
        ServiceLocator.Get<GameManager>().SetErrorManager(this);
    }
   
    private void Quitter()
    {
        var gm = ServiceLocator.Get<GameManager>();
        Destroy(gm.gameObject);
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void Error(string message)
    {
        Debug.Log(message);
        text.text = message;
        if(gameObject != null)
            gameObject.SetActive(true);
    }
}
