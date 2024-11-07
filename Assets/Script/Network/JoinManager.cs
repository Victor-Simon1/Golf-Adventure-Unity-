using UnityEngine;
using Services;

public class JoinManager : MonoRegistrable
{
    [Header("Variables")]
    private string playerName;
    private string sessionName;
    private string ip;
    [Header("Manager")]
    [SerializeField] private GameManager gm;
    [Header("Gameobject")]
    [SerializeField] private GameObject waiting;
    [SerializeField] private GameObject next;

#region UNITY_FUNCTION
    // Start is called before the first frame update
    void Start()
    {
        playerName = "Player ";
        ServiceLocator.Register<JoinManager>(this, false);
        gm = ServiceLocator.Get<GameManager>();
    }

#endregion

#region GETTER_SETTER
    public void SetName(string newName)
    {
        playerName = newName;
    }
    public string GetPlayerName()
    {
        return playerName;
    }
    public void SetSessionName(string sessionName)
    {
        this.sessionName = sessionName;
    }

    public void SetIp(string newIp)
    {
        ip = newIp;
    }

#endregion

#region PUBLIC_FUNCTION
    public void Connect()
    {
        waiting.SetActive(true);
        gm.Connection(ip);
    }

    public void Create()
    {
        gm.CreateParty(sessionName);
    }

    public void StopConnection()
    {
        waiting.SetActive(false);
        gm.StopConnection();
    }

    public void Connected()
    {
        waiting.SetActive(false);
        gameObject.SetActive(false);
        next.SetActive(true); 
    }
#endregion

}
