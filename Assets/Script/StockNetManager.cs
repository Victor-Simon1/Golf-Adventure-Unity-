using Services;
using UnityEngine;
using System;
using Mirror;

public class StockNetManager : MonoRegistrable
{
    [SerializeField] private NetworkManager m;

    // Start is called before the first frame update
    public void Awake()
    {
        ServiceLocator.Register<StockNetManager>(this);
    }

    public NetworkManager GetNetworkManager()
    {
        return m;
    }
}
