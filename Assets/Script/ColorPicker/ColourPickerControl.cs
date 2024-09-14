using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Services;

public class ColourPickerControl : MonoBehaviour
{

    public float currentHue, currentSat, currentVal;

    [SerializeField]
    private RawImage hueImage, satValImage, outputImage;

    [SerializeField]
    private Slider hueSlider;

    private Texture2D hueTexture, svTexture, outputTexture;

    [SerializeField]
    MeshRenderer changeThisColor;
    Color finalColor;

    private void Start()
    {
        CreateHueImage();

        CreateSVImage();

        UpdateOutput();
    }

    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        for (int i = 0; i < hueTexture.height; i++)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height,1f, 0.8f));
        }

        hueTexture.Apply();
        currentHue = 0;

        hueImage.texture = hueTexture;
    }
    
    private void CreateSVImage()
    {
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValueTexture";

        for (int x = 0; x < svTexture.height; x++)
        {
            for (int y = 0; y < svTexture.width; y++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        currentSat = 0;
        currentVal = 0;

        satValImage.texture = svTexture;
    }
    
    private void UpdateOutput()
    {
        Color currentColor = Color.HSVToRGB(currentHue, currentSat, currentVal);

        changeThisColor.material.SetColor("_Color", currentColor);
        finalColor = currentColor;
    }

    public void SetSV(float S,  float V)
    {
        currentSat = S;
        currentVal = V;

        UpdateOutput();
    }

    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;
        for(int y = 0; y < svTexture.height; y++)
        {
            for(int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        UpdateOutput();

    }

    public void ChangeColorPlayer()
    {
        ServiceLocator.Get<GameManager>().GetLocalPlayer().SetColor(finalColor);
    }

}
