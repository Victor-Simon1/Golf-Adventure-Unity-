using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private RotatingSprite rotatingCircle;
    [SerializeField] private RotatingSprite ballSprite;

    [SerializeField] private Image loadingBar;


    public void RotateAll()
    {
        rotatingCircle.Rotate();
        ballSprite.Rotate();
    }

    public void SetProgression(float progression)
    {
        loadingBar.fillAmount = progression;
    }
}
