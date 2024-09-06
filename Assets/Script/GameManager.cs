using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

using System.Linq;
public class GameManager : MonoBehaviour
{
    private const string playerIDprefix = "Player";
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
        

    public static void RegisterPlayer(string netID,Player player)
    {
        Debug.Log(netID + " s'est enregistré");
        string playerID = playerIDprefix + netID;
        players.Add(playerID, player);
        player.transform.name = playerID;
    }

    public static void UnregisterPlayer(string playerID) 
    {
        players.Remove(playerID);
    }
    public static Player GetPlayer(string playerId)
    {
        return players[playerId];
    }
    public static Player[] GetAllPlayer()
    {
        return players.Values.ToArray();
    }

}
