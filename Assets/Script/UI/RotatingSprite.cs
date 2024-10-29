using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotatingSprite : MonoBehaviour
{
    private bool isRotating;
    [SerializeField] private Vector3 step;

    public void Rotate()
    {
        transform.Rotate(step);
    }
}
