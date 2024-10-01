using Services;
using UnityEngine;

public class UIManager : MonoRegistrable
{

    [SerializeField] private PlayerUI playerUI;

    private void Awake()
    {
        ServiceLocator.Register<UIManager>(this);
    }

    public PlayerUI GetPlayerUI()
    {
        return playerUI;
    }
}
