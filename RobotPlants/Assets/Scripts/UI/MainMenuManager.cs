
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    #region Variables

    [Header("Username")]
    string username;
    bool usernameIsEmpty;

    [Header("Object/Component References and Prefabs")]
    TMP_InputField usernameInputField;
    Button hostSessionButton;
    Button joinSessionButton;
    Button dedicatedButton;
    Button localButton;
    Button creditsButton;
    Button exitGameButton;
    GameObject creditsPanel;
    GameObject joinButtonsPanel;

    [Header("Object/Component References")]
    NetManager networkManager;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        networkManager = FindObjectOfType<NetManager>();

        //Get TMPInputField ref
        usernameInputField = GetComponentInChildren<TMP_InputField>();

        //Get button refs
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if(buttons[i].gameObject.name.Equals("HostSessionButton")) hostSessionButton = buttons[i];
            else if(buttons[i].gameObject.name.Equals("JoinSessionButton")) joinSessionButton = buttons[i];
            else if(buttons[i].gameObject.name.Equals("DedicatedSessionButton")) dedicatedButton = buttons[i];
            else if(buttons[i].gameObject.name.Equals("LocalSessionButton")) localButton = buttons[i];
            else if(buttons[i].gameObject.name.Equals("CreditsButton")) creditsButton = buttons[i];
            else if(buttons[i].gameObject.name.Equals("ExitGameButton")) exitGameButton = buttons[i];
        }

        //Get button refs
        Image[] images = GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].gameObject.name.Equals("CreditsPanel")) creditsPanel = images[i].gameObject;
            if (images[i].gameObject.name.Equals("JoinButtonsPanelVert")) joinButtonsPanel = images[i].gameObject;
        }

        //Add functions to UI elements
        usernameInputField.onValueChanged.AddListener(delegate { OnUsernameChanged(); });
        hostSessionButton.onClick.AddListener(delegate { HostSession(); });
        joinSessionButton.onClick.AddListener(delegate { JoinSession(); });
        dedicatedButton.onClick.AddListener(delegate { JoinDedicatedSession(); });
        localButton.onClick.AddListener(delegate { JoinLocalSession(); });
        creditsButton.onClick.AddListener(delegate { ToggleCredits(); });
        exitGameButton.onClick.AddListener(delegate { ExitGame(); });

        //Starts buttons uninteractable
        hostSessionButton.interactable = false;
        joinSessionButton.interactable = false;
        dedicatedButton.interactable = false;
        localButton.interactable = false;

        //Start creditsPanel inactive
        creditsPanel.SetActive(false);

        //Start joinButtonsPanelVert inactive
        joinButtonsPanel.SetActive(false);

        if(PlayerPrefs.HasKey("name"))
        {
            usernameInputField.text = LoadName();
        }

    }

    #endregion

    #region Custom Methods

    #region Username

    public void OnUsernameChanged()
    {
        //Update username value
        username = usernameInputField.text;

        //Check if username is empty
        usernameIsEmpty = (username.Length == 0);

        //Enable/disable buttons based on if username is empty or not
        hostSessionButton.interactable = !usernameIsEmpty;
        joinSessionButton.interactable = !usernameIsEmpty;
        dedicatedButton.interactable = !usernameIsEmpty;
        localButton.interactable = !usernameIsEmpty;
    }

    public bool IsUsernameEmpty() { return usernameIsEmpty; }

    #endregion

    #region Button Functions

    public void HostSession()
    {
        //Add code here to host server and connect as client
        if(!IsUsernameEmpty())
        {
            DisableAllUI();

            SaveName(usernameInputField.text);
            networkManager.StartHost();

            //CODE GOES HERE
            Debug.Log("HOST SESSION");
        }
    }

    public void JoinSession()
    {
        if (!IsUsernameEmpty())
        {
            SaveName(usernameInputField.text);
            //Toggle joinButtonsPanelVert
            joinButtonsPanel.SetActive(!joinButtonsPanel.activeInHierarchy);
        }
    }

    public void JoinDedicatedSession()
    {
        //Add code here to connect as client to dedicated server
        if (!IsUsernameEmpty())
        {
            DisableAllUI();

            SaveName(usernameInputField.text);
            networkManager.StartClient();
            //CODE GOES HERE
            Debug.Log("JOIN SESSION");
        }
    }

    public void JoinLocalSession()
    {
        //Add code here to connect as client to local server
        if (!IsUsernameEmpty())
        {
            DisableAllUI();

            SaveName(usernameInputField.text);
            // networkManager.networkAddress = ;
            networkManager.networkAddress = "localhost";
            networkManager.StartClient();
            //CODE GOES HERE
            Debug.Log("JOIN SESSION");
        }
    }

    public void ToggleCredits()
    {
        creditsPanel.SetActive(!creditsPanel.activeInHierarchy);
        Debug.Log("Toggle Credits");
    }

    public void ExitGame()
    {
        Debug.Log("EXIT GAME");
        Application.Quit(0);
    }

    public void SaveName(string name)
    {
        PlayerPrefs.SetString("name", usernameInputField.text);
        PlayerPrefs.Save();
    }
    public string LoadName()
    {
        return PlayerPrefs.GetString("name", usernameInputField.text);
    }

    void DisableAllUI()
    {
        hostSessionButton.interactable = false;
        joinSessionButton.interactable = false;
        dedicatedButton.interactable = false;
        localButton.interactable = false;
        creditsButton.interactable = false;
        exitGameButton.interactable = false;

        usernameInputField.interactable = false;
    }

    #endregion

    #endregion
}
