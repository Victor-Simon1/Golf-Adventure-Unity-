using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleBehavior : MonoBehaviour 
{
    [SerializeField] private int numberHole;
    public static int nbHole = 10;

    private void Start()
    {
      
    }
  
    private void OnEnable()
    {
        ServiceLocator.Get<GameManager>().AddHole(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Prevenir le joueur qu'il est rentré dans le trou
        }
    }
}
