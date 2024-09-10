using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleBehavior : MonoBehaviour
{
    [SerializeField] private int numberHole;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //Prevenir le joueur qu'il est rentré dans le trou
        }
    }
}
