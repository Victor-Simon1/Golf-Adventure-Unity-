using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using System;
public class StartBehavior : MonoBehaviour, IComparable
{
    [SerializeField] private int numberHole;
    public static int nbHole = 10;

    private void Start()
    {
        if (numberHole == 1)
        {
            ServiceLocator.Get<GameManager>().TeleportToPoint(numberHole - 1);
        }
    }
    private void OnEnable()
    {
        ServiceLocator.Get<GameManager>().AddStart(this);
    }
    public int CompareTo(object obj)
    {
        var a = this;
        var b = obj as StartBehavior;

        if (a.numberHole < b.numberHole)
            return -1;

        if (a.numberHole > b.numberHole)
            return 1;

        return 0;
    }
}
