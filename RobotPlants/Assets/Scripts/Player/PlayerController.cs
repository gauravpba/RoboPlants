using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.Users;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [Header("Input")]
    private PlayerInputActions controls = null; //Reference to the Input System Asset "Controls"

    [Header("Input Device Type")]
    //[HideInInspector] public InputDeviceType currentInputDeviceType;
    [HideInInspector] public UnityEvent OnInputDeviceChange; //This event is called when the input device is changed

    [Header("Object/Component References and Prefabs")]
    [HideInInspector] public PlayerBody playerBody; //The body of the controlled character
    [HideInInspector] public PlayerCameraController playerCameraController; //The player's camera
    [HideInInspector] public PlayerUIManager playerUIManager; //The UI manager component

    [Header("Input Polling")]
    Vector2 moveInput; //The move input as read from controls
    Vector2 tileTargetingInput; //The targeting input as read from controls
    Vector2 cameraInput; //The camera input as read from controls

    #endregion

    #region Unity Methods

    //public static PlayerController instance = null; //Singleton object reference

    private void Awake()
    {
        //#region Singleton

        ////If instance doesn't already exist
        //if (instance == null)
        //{
        //    Debug.Log("No singleton instance found!");

        //    //Set instance to this
        //    instance = this;
        //}
        //else if (instance != this) //Else if instance already exists and it's not this
        //{
        //    Debug.Log("Singleton instance found, destroying PlayerController!");

        //    //Destroy this
        //    Destroy(gameObject);
        //}

        //#endregion

        InitializeInputControls();

        ////TODO: Test if PlayerInput is necessary for input type change detection and remove if unnecessary--------------------------------
        //PlayerInput input = GetComponent<PlayerInput>();
        //UpdateDeviceInterfaceType(input.currentControlScheme);
    }

    public void Start()
    {
        SubscribeInputControls();

        //Enable player controls
        SwitchActionMap("Player");

    }

  
    private void OnDestroy()
    {
        UnsubscribeInputControls();
    }

    private void Update()
    {
        PollForUpdateInputs();

        ////Pass the camera input through to the camera controller
        //playerCameraController.RotateCamera(cameraInput);

        //Pass the horizontal component of movement input through to the character
        playerBody.SetMovementInput(moveInput.x);
    }

    #endregion

    #region Custom Methods

    #region Initialization

    void InitializeInputControls()
    {
        if (controls == null) controls = new PlayerInputActions(); //Grab reference to Input System Asset "Controls"
    }

    #endregion

    #region Input Controls

    void SubscribeInputControls()
    {
        #region Player Controls
        controls.Player.Jump.started += context => playerBody.Jump();

        controls.Player.SwapItem.started += context => playerBody.SwapItem();
        controls.Player.UseItem.started += context => playerBody.UseItem();
        controls.Player.RemoveItem.started += context => playerBody.RemoveItem();

        controls.Player.Water.started += context => playerBody.Water();

        controls.Player.Pause.started += context => playerUIManager.Pause();

        //InputUser.onChange += OnInputUserDeviceChange;
        #endregion

        #region UI Controls

        #endregion
    }

    void UnsubscribeInputControls()
    {
        #region Player Controls
        controls.Player.Jump.started -= context => playerBody.Jump();

        controls.Player.SwapItem.started -= context => playerBody.SwapItem();
        controls.Player.UseItem.started -= context => playerBody.UseItem();
        controls.Player.RemoveItem.started -= context => playerBody.RemoveItem();

        controls.Player.Water.started -= context => playerBody.Water();

        controls.Player.Pause.started -= context => playerUIManager.Pause();

        //InputUser.onChange -= OnInputUserDeviceChange;
        #endregion

        #region UI Controls

        #endregion
    }

    public void SwitchActionMap(string mapName)
    {
        controls.Player.Disable();
        controls.UI.Disable();

        switch(mapName)
        {
            default:
            case "Player":
                controls.Player.Enable();
                break;

            case "UI":
                controls.UI.Enable();
                break;
        }
    }

    //#region Device Changing

    ////Method called when the device being used changes
    //void OnInputUserDeviceChange(InputUser user, InputUserChange change, InputDevice device)
    //{
    //    if (change == InputUserChange.ControlSchemeChanged)
    //    {
    //        UpdateDeviceInterfaceType(user.controlScheme.Value.name, device);
    //    }
    //}

    ////Change whether KBM, XB, or PS control interface should be shown
    //void UpdateDeviceInterfaceType(string schemeName, InputDevice newDevice = null)
    //{
    //    //Cases for control schemes: KeyboardMouse and Gamepad
    //    if (schemeName.Equals("KeyboardMouse")) currentInputDeviceType = InputDeviceType.KeyboardMouse;
    //    else if (schemeName.Equals("DualshockGamepad")) currentInputDeviceType = InputDeviceType.DualshockGamepad;
    //    else if (schemeName.Equals("XInputGamepad")) currentInputDeviceType = InputDeviceType.XInputGamepad;

    //    //Debug.Log("New control scheme type: " + System.Enum.GetName(typeof(InputDeviceType), currentInputDeviceType));

    //    //Tell other scripts that are listening for changes to update their interface
    //    OnInputDeviceChange.Invoke();
    //}

    ////Returns the current input device type
    //public InputDeviceType GetCurrentInputDeviceType()
    //{
    //    return currentInputDeviceType;
    //}

    //#endregion

    void PollForUpdateInputs()
    {
        //cameraInput = controls.Player.CameraInput.ReadValue<Vector2>(); //Get camera input
        moveInput = controls.Player.MoveInput.ReadValue<Vector2>(); //Get movement input

        tileTargetingInput = controls.Player.TileTargeting.ReadValue<Vector2>(); //Get mouse position input
        Vector3 tilePosition = playerCameraController.activeCam.ScreenToWorldPoint(new Vector3(tileTargetingInput.x, tileTargetingInput.y, 0f));
        playerBody.SetTargetedTile((int)Mathf.Floor(tilePosition.x), (int)Mathf.Floor(tilePosition.y));
        //Debug.Log("tti: " + "( " + Mathf.Floor(pos.x) + ", " + Mathf.Floor(pos.y) + " )");
    }

    #endregion

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 cubePosition = playerCameraController.activeCam.ScreenToWorldPoint(new Vector3(tileTargetingInput.x, tileTargetingInput.y, 0f));
        Gizmos.DrawWireCube(new Vector3(Mathf.Floor(cubePosition.x) + 0.5f, Mathf.Floor(cubePosition.y) + 0.5f, 0f), Vector3.one);

    }
#endif

}

//#region Data Structures

////Used to track the type of the currently used input device
//public enum InputDeviceType
//{
//    KeyboardMouse,
//    XInputGamepad,
//    DualshockGamepad
//}

//#endregion

