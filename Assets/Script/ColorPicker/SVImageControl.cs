using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SVImageControl : MonoBehaviour,IDragHandler,IPointerClickHandler
{
    [SerializeField] private Image pickerImage;
    private RawImage SVImage;

    [SerializeField] private ColourPickerControl cc;

    private RectTransform rectTransform, pickerTransform;

  
    private void Awake()
    {
        SVImage = GetComponent<RawImage>();
       // cc = GetComponent<ColourPickerControl>();
        rectTransform = GetComponent<RectTransform>();

        pickerTransform = pickerImage.GetComponent<RectTransform>();
        pickerTransform.position = new Vector2(-(rectTransform.sizeDelta.x * 0.5f), -(rectTransform.sizeDelta.y * 0.5f));

    }

    void UpdateColour(PointerEventData eventData)
    {
        Vector3 pos = rectTransform.InverseTransformPoint(eventData.position); 

        float deltaX = rectTransform.sizeDelta.x * 0.5f;
        float deltaY = rectTransform.sizeDelta.y * 0.5f;

        if (pos.x < -deltaX)
            pos.x = -deltaX;
        else if(pos.x > deltaX)
            pos.x = deltaX;

        if (pos.y < -deltaY)
            pos.y = -deltaY;
        else if (pos.y > deltaY)
            pos.y = deltaY;

        float x = pos.x + deltaX;
        float y = pos.y + deltaY;


        float xNorm = x / rectTransform.sizeDelta.x;
        float yNorm = y / rectTransform.sizeDelta.y;

        pickerTransform.localPosition = pos;
        pickerImage.color = Color.HSVToRGB(0, 0, 1 - yNorm);

        cc.SetSV(xNorm, yNorm);
    }


    public void OnDrag(PointerEventData eventData)
    {
       UpdateColour(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UpdateColour(eventData);
    }

}
