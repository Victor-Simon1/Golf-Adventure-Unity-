using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{

    public static List<GameObject> dontDestroyTargets = new List<GameObject>();
    private void Awake()
    {
        dontDestroyTargets.Add(gameObject);
        DontDestroyOnLoad(this);
    }
}
