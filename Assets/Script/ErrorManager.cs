using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Services;

public class ErrorManager : MonoBehaviour
{
    [Header("Gameobject")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button quitter;

#region UNITY_FUNCTION
    private void Start()
    {
        quitter.onClick.AddListener(Quitter);
        ServiceLocator.Get<GameManager>().SetErrorManager(this);
    }
#endregion

#region PRIVATE_FUNCTION
    private void Quitter()
    {
        var gm = ServiceLocator.Get<GameManager>();
        DontDestroy.dontDestroyTargets.ForEach(t => { Destroy(t); });
        Destroy(gm.gameObject);
        SceneManager.LoadScene("MenuPrincipal");
    }
#endregion

#region PUBLIC_FUNCTION
    public void Error(string message)
    {
        Debug.LogError(message);
        text.text = message;
        if(gameObject != null)
            gameObject.SetActive(true);
    }
#endregion
}
