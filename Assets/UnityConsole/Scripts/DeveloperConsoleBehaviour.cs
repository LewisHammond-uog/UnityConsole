using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lewis.DevConsole;

/// <summary>
/// Monobehaviour to run the developer console,
/// deals with UI display
/// </summary>
public class DeveloperConsoleBehaviour : MonoBehaviour
{
    //Prefix for commands
    [Header("Commands")]
    [SerializeField] private string commandPrefix = "/";


    [Header("Settings")]
#if ENABLE_LEGACY_INPUT_MANAGER
    [SerializeField] private KeyCode toggleKey = KeyCode.Tilde;
#endif

    //If we should pause the game while the console is up
    [SerializeField] public bool ConsolePausesGame { get; set; } = false;
    private float runningTimescale; //timescale of the game when we opened the console

    //Bool for if the console is active or not
    private bool isConsoleActive = false;

    [Header("UI")]
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private TMP_InputField inputFeild;
    [SerializeField] private TMP_Text outputText;


    //Instances of the console & UI
    private static DeveloperConsoleBehaviour behaviourInstance;
    private static DeveloperConsole consoleInstance;
    private DeveloperConsole DeveloperConsole
    {
        get
        {
            consoleInstance = consoleInstance ?? new DeveloperConsole(commandPrefix, null);
            return consoleInstance;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        //Check console UI does not already exist, if it does then delete this
        if(behaviourInstance != null && behaviourInstance != this)
        {
            Destroy(this);
            return;
        }

        //Set this as the behaviour instance
        behaviourInstance = this;

        //Stop console being destroyed on load
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        //Get if the ui is enabled or not
        isConsoleActive = uiCanvas.enabled;
    }

    private void Update()
    {
        //If we are using the old input system with old polling method
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleUI();
        }        
#endif
    }

    /// <summary>
    /// Toggles if the UI is visible or not
    /// </summary>
    private void ToggleUI()
    {
        //Toggle active state
        isConsoleActive = !isConsoleActive;

        //If we pause the game when the console is active then pause/unpause the game]
        if (ConsolePausesGame)
        {
            //Console has just been activated save the running time scale
            if (isConsoleActive)
            {
                runningTimescale = Time.timeScale;
            }
            
            Time.timeScale = isConsoleActive ? 0 : runningTimescale;
        }

        uiCanvas.enabled = isConsoleActive;
        if (isConsoleActive) { inputFeild.ActivateInputField(); }
    }

    /// <summary>
    /// Takes input from the text box and sends it to the developer console 
    /// for processing
    /// </summary>
    public void ProcessInput(string input)
    {
        DeveloperConsole.ProcessConsoleInput(input);
        inputFeild.text = string.Empty;
    }
}
