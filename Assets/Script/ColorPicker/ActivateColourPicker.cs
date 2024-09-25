using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateColourPicker : MonoBehaviour
{
    //Canvas of the colour picker 
    [SerializeField] GameObject colourPicker;

    //Enable or Unable the canvas whern button is clic
    public void ChangeStateColourPicker()
    {
        if(colourPicker.activeSelf)
            colourPicker.SetActive(false);
        else 
            colourPicker.SetActive(true);
    }
}
