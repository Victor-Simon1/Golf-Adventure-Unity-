using Services;
using UnityEngine;
using Mirror;

public class StockNetManager : MonoRegistrable
{
    [Header("Script")]
    [SerializeField] private NetworkManager m;

#region UNITY_FUNCTION
    // Start is called before the first frame update
    public void Awake()
    {
        ServiceLocator.Register<StockNetManager>(this, false);
    }
    #endregion

#region GETTER_SETTER
    public NetworkManager GetNetworkManager()
    {
        return m;
    }
#endregion
}
