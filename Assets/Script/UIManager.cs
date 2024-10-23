using Services;
using UnityEngine;

public class UIManager : MonoRegistrable
{
    [Header("Script")]
    [SerializeField] private PlayerUI playerUI;

#region UNITY_FUNCTION
    private void Awake()
    {
        ServiceLocator.Register<UIManager>(this);
    }

    private void OnEnable()
    {
        ServiceLocator.Get<GameManager>().setUIManager(this);
    }
#endregion

#region GETTER_SETTER
    public PlayerUI GetPlayerUI()
    {
        return playerUI;
    }
#endregion
}
