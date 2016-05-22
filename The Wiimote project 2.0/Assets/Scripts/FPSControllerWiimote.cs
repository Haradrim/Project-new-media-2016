using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WiimoteApi;
using WiimoteApi.Util;


public class FPSControllerWiimote : MonoBehaviour
{

    private Wiimote wiimote;
    private NunchuckData nunchuckData;

    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpSpeed = 10f;
    public float downForce = 10f;
    public Rigidbody player;
    public List<bool> playerLeds;
    public Camera playerCam;
    public RectTransform IR_pointer;
    public bool setupCompleted { get; set; }

    private bool initIsPressed = false;
    private bool jump;
    private bool isInit = false;
    private bool isJumping = false;
    private bool isWalking = true;
    private bool isRunning = false;
    private bool isAirborn;
    private Vector2 currentVelocity;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        setupCompleted = false;
        characterController = GetComponent<CharacterController>();
        isInit = false;
    }

    void Update()
    {
        //Connectie wiimote & IR basic setup
        WiimoteManager.FindWiimotes();
        if (!WiimoteManager.HasWiimote()) { return; }
        wiimote = WiimoteManager.Wiimotes[0];
        if (!isInit)
        {
            wiimote.SendPlayerLED(playerLeds[0], playerLeds[1], playerLeds[2], playerLeds[3]);
            wiimote.SetupIRCamera(IRDataType.BASIC);
            isInit = true;
        }

        int response;
        do
        {
            response = wiimote.ReadWiimoteData();
            if (response < 0) { Debug.Log("Error: " + response); }
        } while (response > 0);

        ReadOnlyMatrix<int> ir = wiimote.Ir.ir;
        int dotCount = 4;
        for (int i = 0; i < 4; i++)
        {
            if (ir[i, 0] == -1 || ir[i, 1] == -1)
            {
                dotCount--;
            }
        }
        if (dotCount < 2) { return; }

        //Wiimote camera movement
        float[] pointer = wiimote.Ir.GetPointingPosition();
        Vector2 curAnchorMin = IR_pointer.anchorMin;
        Vector2 curAnchorMax = IR_pointer.anchorMax;

        IR_pointer.anchorMin = Vector2.SmoothDamp(curAnchorMin, new Vector2(pointer[0], pointer[1]), ref currentVelocity, 0.1f, 1f);
        IR_pointer.anchorMax = Vector2.SmoothDamp(curAnchorMax, new Vector2(pointer[0], pointer[1]), ref currentVelocity, 0.1f, 1f);

        if (setupCompleted)
        {
            playerCam.transform.LookAt(IR_pointer);

            if (wiimote != null && wiimote.current_ext != ExtensionController.NONE)
            {
                if (wiimote.current_ext == ExtensionController.NUNCHUCK)
                {
                    nunchuckData = wiimote.Nunchuck;
                    isRunning = nunchuckData.z;
                }
            }

            //Speler status
            if (!jump)
            {
                if (wiimote.Button.b) { jump = true; }
            }
            if (!isAirborn && characterController.isGrounded)
            {
                //Speler land
                moveDirection.y = 0f;
                isJumping = false;
            }
            if (!characterController.isGrounded && !isJumping && isAirborn)
            {
                //Speler naar beneden forceren
                moveDirection.y = 0f;
            }

            isAirborn = characterController.isGrounded;

        }

    }

    void FixedUpdate()
    {
        if (wiimote == null && setupCompleted) { return; }

        //Speler bewegings controls
        float speed = 0;
        if (isWalking) { speed = walkSpeed; }
        if (isRunning) { speed = runSpeed; }

        if (wiimote != null && wiimote.current_ext != ExtensionController.NONE)
        {
            if (wiimote.current_ext == ExtensionController.NUNCHUCK)
            {
                nunchuckData = wiimote.Nunchuck;
                Vector3 desiredDirection = playerCam.transform.right * NunchuckMovement(nunchuckData.stick[0]) + playerCam.transform.forward * NunchuckMovement(nunchuckData.stick[1]);
                moveDirection.x = desiredDirection.x * speed;
                moveDirection.z = desiredDirection.z * speed;

                if (characterController.isGrounded)
                {
                    moveDirection.y = -downForce;
                    if (jump)
                    {
                        moveDirection.y = jumpSpeed;
                        jump = false;
                        isJumping = true;
                    }
                }
                else
                {
                    moveDirection += Physics.gravity * 2f * Time.fixedDeltaTime;
                }
                characterController.Move(moveDirection * Time.fixedDeltaTime);

            }
        }
    }

    void OnGUI()
    {
        if (wiimote == null) { return; }

        if (!setupCompleted)
        {
            if (wiimote.Button.b && wiimote.Button.a)
            {
                //RUMBLE
                wiimote.RumbleOn = true;
                wiimote.SendStatusInfoRequest();
                wiimote.RumbleOn = false;
                initIsPressed = true;
            }
            else
            {
                if (initIsPressed)
                {

                    setupCompleted = true;
                    initIsPressed = false;
                }
            }
        }

    }

    float NunchuckMovement(float data)
    {
        if (data >= 100 && data < 150)
        {
            return 0;
        }
        else if (data >= 200 && data > 150)
        {
            return 1;
        }
        else if (data < 100)
        {
            return -1;
        }
        return 0;
    }
}
