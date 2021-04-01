using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIManager : MonoBehaviour
{
    #region Variables

    [Header("Pausing")]
    bool gamePaused = false;

    [Header("Object/Component References and Prefabs")]
    GameObject playerPauseMenuCanvasObject;
    Button resumeButton;
    Button mainMenuButton;
    [HideInInspector] public PlayerBody playerBody; //Reference to the playerBody <--- Set by the playerBody in instantiation
    //[HideInInspector] public Camera playerCamera; //Reference to the playerCamera <--- Set by the playerBody in instantiation

    #endregion

    #region Unity Methods

    private void Awake()
    {
        //Get canvas object
        playerPauseMenuCanvasObject = GetComponentInChildren<Canvas>().gameObject;

        //Get button refs
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject.name.Equals("ResumeButton")) resumeButton = buttons[i];
            else if (buttons[i].gameObject.name.Equals("MainMenuButton")) mainMenuButton = buttons[i];
        }

        //Add functions to UI elements
        resumeButton.onClick.AddListener(delegate { Pause(); });
        mainMenuButton.onClick.AddListener(delegate { ReturnToMainMenu(); });
    }

    private void Start()
    {
        //Initialize menu to unpaused
        OnResume();
    }

    #endregion

    #region Custom Methods

    #region Button Functions

    //Called by the PlayerController to pause and unpause
    public void Pause()
    {
        //Toggle pause
        if (gamePaused) OnResume();
        else OnPause();
    }

    void ReturnToMainMenu()
    {
        //TODO: Disconnect from server and return to main menu ---------------------------

    }

    #endregion

    #region Pause Functions

    void OnPause()
    {
        //Update pause State
        gamePaused = true;

        //Switch Control Scheme to UI
        playerBody.playerController.SwitchActionMap("UI");

        //Show pause menu
        playerPauseMenuCanvasObject.SetActive(true);
    }

    void OnResume()
    {
        //Update pause State
        gamePaused = false;

        //Switch Control Scheme to Player
        playerBody.playerController.SwitchActionMap("Player");

        //Hide pause menu
        playerPauseMenuCanvasObject.SetActive(false);
    }

    #endregion

    #endregion
}
