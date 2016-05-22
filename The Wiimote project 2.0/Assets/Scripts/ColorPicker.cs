using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class ColorPicker : MonoBehaviour {

    public Drawing drawingScript;
    public EventSystem eventSystem;
    public Texture brush;
    public RectTransform colorPreview;
    public Material paint;

    private float r = 0f;
    private float g = 0f;
    private float b = 0f;
    private float a = 1f;

    public Slider sliderR;

    void Start ()
    {
        SelectFirstSlider();
    }
	
	void Update ()
    {
        Slider selectSlider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
        if(selectSlider != null)
        {
            selectSlider.Select();
        }

        Color newColor = new Color(r, g, b, a);
        colorPreview.GetComponent<Image>().color = newColor;
        if (paint.GetColor("_Color") != newColor)
        {
            Material newMaterial = createNewMaterial(newColor);
            drawingScript.SetBrush(newMaterial);

        }
        else
        {
            paint.SetColor("_Color", newColor);
        }

    }
    Material createNewMaterial(Color newColor)
    {
        Material newMaterial = new Material(Shader.Find("Standard"));

        //Rendering Mode
        newMaterial.SetFloat("_Mode", 2);
        newMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        newMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        newMaterial.SetInt("_ZWrite", 0);
        newMaterial.DisableKeyword("_ALPHATEST_ON");
        newMaterial.EnableKeyword("_ALPHABLEND_ON");
        newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        newMaterial.renderQueue = 3000;
        newMaterial.color = newColor;
        newMaterial.SetTexture("_MainTex", brush);
        return newMaterial;

    }
    public void NextSlider()
    {
        Slider curSlider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
        if (curSlider != null && curSlider.FindSelectable(Vector3.up) != null)
        {
            curSlider.FindSelectable(Vector3.up).Select();
        }
    }
    public void PrevSlider()
    {
        Slider curSlider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
        if(curSlider != null && curSlider.FindSelectable(Vector3.down) != null)
        {
            curSlider.FindSelectable(Vector3.down).Select();
        }
    }

    public void IncreaseValueSlider()
    {
        Slider curSlider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
        curSlider.value += 0.025f;
    }
    public void DecreaseValueSlider()
    {
        Slider curSlider = eventSystem.currentSelectedGameObject.GetComponent<Slider>();
        curSlider.value -= 0.025f;
    }

    public void updateR(float newR)
    {
        r = newR;
    }
    public void updateG(float newG)
    {
        g = newG;
    }
    public void updateB(float newB)
    {
        b = newB;
    }
    public void updateAlpha(float newA)
    {
        a = newA;
    }
    public void SelectFirstSlider()
    {
        sliderR.Select();
    }
}
