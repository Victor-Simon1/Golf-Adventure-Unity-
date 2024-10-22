using UnityEngine;
using UnityEngine.EventSystems;

public class SliderTouch : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    [Header("Variables")]
    public bool isPressed;

#region IPOINTER_FUNCTION
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
#endregion
}
