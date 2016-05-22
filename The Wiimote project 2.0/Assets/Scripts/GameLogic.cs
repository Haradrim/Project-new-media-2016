using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using WiimoteApi;
using WiimoteApi.Util;

public class GameLogic : MonoBehaviour
{

    private Wiimote wiimote;
    private GameObject connectWiimote;
    private GameObject startScreen;

    public FPSControllerWiimote FPSControllerScript;
    public Drawing drawingScript;
    public ColorPicker colorPickerScript;

    public GameObject uiPanel;
    public GameObject uiPointer;
    public GameObject colorPicker;

    private bool minusPressed;
    private bool plusPressed;
    private bool aPressed;
    private bool upPressed;
    private bool downPressed;

    void Start()
    {

        //colorPicker.SetActive(false);

        if (!connectWiimote)
        {
            connectWiimote = GameObject.Find("ConnectWiimote");
        }
        if (!startScreen)
        {
            startScreen = GameObject.Find("StartGame");
            startScreen.SetActive(false);
        }
    }

    void Update()
    {

        WiimoteManager.FindWiimotes();
        if (!WiimoteManager.HasWiimote()) { return; }
        wiimote = WiimoteManager.Wiimotes[0];

        if (FPSControllerScript.setupCompleted)
        {

            if (wiimote.Button.a)
            {
                drawingScript.StartDrawing();
                aPressed = true;
            }
            else if (!wiimote.Button.a)
            {
                if (aPressed)
                {
                    drawingScript.StopDrawing();
                }
            }
            if (wiimote.Button.minus)
            {
                minusPressed = true;
            }
            else if (!wiimote.Button.minus)
            {
                if (minusPressed)
                {
                    drawingScript.Undo();
                    minusPressed = false;
                }
            }
            if (wiimote.Button.plus)
            {
                plusPressed = true;
            }
            else if (!wiimote.Button.plus)
            {
                if (plusPressed)
                {
                    drawingScript.Redo();
                    plusPressed = false;
                }
            }

            if (wiimote.Button.d_down)
            {
                downPressed = true;
            }
            if (!wiimote.Button.d_down)
            {
                if (downPressed)
                {
                    colorPickerScript.PrevSlider();
                    downPressed = false;
                }
            }
            if (wiimote.Button.d_up)
            {
                upPressed = true;
            }
            if (!wiimote.Button.d_up)
            {
                if (upPressed)
                {
                    colorPickerScript.NextSlider();
                    upPressed = false;
                }
            }
            if (wiimote.Button.d_left)
            {
                colorPickerScript.DecreaseValueSlider();
            }
            if (wiimote.Button.d_right)
            {
                colorPickerScript.IncreaseValueSlider();
            }
        }
    }

    void OnGUI()
    {
        connectWiimote.SetActive(true);
        if (wiimote == null) { return; }
        connectWiimote.SetActive(false);

        if (FPSControllerScript.setupCompleted)
        {
            uiPanel.GetComponent<Image>().enabled = false;
            //uiPointer.GetComponent<Image>().enabled = false;
            startScreen.SetActive(false);
        }
        else
        {
            startScreen.SetActive(true);
        }
    }

}
