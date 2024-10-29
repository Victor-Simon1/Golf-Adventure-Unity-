using Services;
using UnityEngine;

public class UIManager : MonoRegistrable
{
    [Header("Script")]
    [SerializeField] private PlayerUI playerUI;

    [SerializeField] private GameObject HubCanvas;
    [SerializeField] private LoadingScreen loadingScreen;

#region UNITY_FUNCTION
    private void Awake()
    {
        ServiceLocator.Register<UIManager>(this,false);
    }

#endregion

#region GETTER_SETTER
    public void SetPlayerUI(PlayerUI pui)
    {
        playerUI = pui;
    }

    public PlayerUI GetPlayerUI()
    {
        return playerUI;
    }

    public LoadingScreen GetLoadingScreen()
    {
        return loadingScreen;
    }

    public GameObject GetHubCanvas()
    {
        return HubCanvas;
    }
#endregion
}
