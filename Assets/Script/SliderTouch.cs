using Org.BouncyCastle.Asn1.Mozilla;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SliderTouch : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{

    public bool isPressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
