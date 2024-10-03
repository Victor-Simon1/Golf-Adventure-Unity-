using Services;
using UnityEngine;

public class UIManager : MonoRegistrable
{

    [SerializeField] private PlayerUI playerUI;

    private void Awake()
    {
        ServiceLocator.Register<UIManager>(this);
    }

    private void OnEnable()
    {
        ServiceLocator.Get<GameManager>().setUIManager(this);
    }

    public PlayerUI GetPlayerUI()
    {
        return playerUI;
    }
}
