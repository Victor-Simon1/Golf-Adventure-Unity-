using Org.BouncyCastle.Asn1.Mozilla;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SliderTouch : MonoBehaviour,IPointerDownHandler
{

    public bool isPressed;

 
    public void IsPressed()
    {
        isPressed = true;
    }
    public void IsUnPressed()
    {
        isPressed=false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
}
