using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lewis.DevConsole;
using System.Text;

/// <summary>
/// Monobehaviour to run the developer console,
/// deals with UI display
/// </summary>
public class DeveloperConsoleBehaviour : MonoBehaviour
{
    //Prefix for commands
    [Header("Commands")]
    [SerializeField] private string outputPrefix = "<b>></b> ";
    [SerializeField] private string commandPrefix = "/";


    [Header("Settings")]
#if ENABLE_LEGACY_INPUT_MANAGER
    [SerializeField] private KeyCode toggleKey = KeyCode.Tilde;
#endif
    [SerializeField] private bool showUnityLogs = true; //If we should show logs from Debug.Log
    [SerializeField] private bool showUnityStackTrace = true; //If we should show unity stack traces, only shows if we showUnityLogs
    [SerializeField] private bool consolePausesGame = false;    //If we should pause the game while the console is up
    private float runningTimescale; //timescale of the game when we opened the console

    //Properties for settings so they can be changed by commands
    public bool ConsolePausesGame { get { return consolePausesGame; } set { consolePausesGame = value; } }
    public bool ShowUnityLogs { get { return showUnityLogs; } set { showUnityLogs = value; } }
    public bool ShowStackTrace { get { return showUnityStackTrace; } set { showUnityStackTrace = value; } }

    //Bool for if the console is active or not
    private bool isConsoleActive = false;

    [Header("Colours")]
    [SerializeField] private Color infoColour;
    [SerializeField] private Color warningColour;
    [SerializeField] private Color errorColour;

    [Header("UI")]
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private TMP_InputField inputFeild;
    [SerializeField] private TMP_InputField outputText; //we ust an input feild which is read-only so that the text can be selected (and then coppied)

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

        //Make sure the output text is read-only, so our 
        //output cannot be edited
        outputText.readOnly = true;

        //Stop console being destroyed on load
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        //Get if the ui is enabled or not
        isConsoleActive = uiCanvas.enabled;

        //Sub to event to get logs
        Application.logMessageReceived += HandleUnityLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleUnityLog;
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
    /// Add text to the output log
    /// </summary>
    /// <param name="type">Type of Log</param>
    /// <param name="log">Log string</param>
    private void AddOutputLog(LogType type, string log)
    {
        //Add string to the output text
        if(outputText == null) { return; }
        //Get the colour based on the log type
        Color textDrawColour;
        switch (type)
        {
            case LogType.Exception:
            case LogType.Error:
            case LogType.Assert:
                textDrawColour = errorColour;
                break;
            case LogType.Warning:
                textDrawColour = warningColour;
                break;
            case LogType.Log:
                textDrawColour = infoColour;
                break;
            default:
                //Default to white
                textDrawColour = Color.white;
                break;
        }

        StringBuilder outputSB = new StringBuilder();
        outputSB.Append($"<color=#{ColorUtility.ToHtmlStringRGB(textDrawColour)}>");
        outputSB.Append(outputPrefix);
        outputSB.Append(log);
        outputSB.Append("</color>");

        outputText.text += outputSB.ToString();
    }

    /// <summary>
    /// Handle Logs which we recive from the Unity .Log, .LogWarning, .LogError, etc.
    /// </summary>
    /// <param name="logString">Log String</param>
    /// <param name="stackTrace">Log Stack Trace</param>
    /// <param name="type">Type of Log</param>
    private void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        //Only process if we want to output to console
        if (!ShowUnityLogs) { return; }

        //Add to output log
        StringBuilder outputSB = new StringBuilder();
        outputSB.Append("<b>" + logString + "</b>");
        if (ShowStackTrace)
        {
            outputSB.Append("\n" + stackTrace);
        }
        outputSB.AppendLine(); //blank line
        AddOutputLog(type, outputSB.ToString());
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
    public void ProcessInput(TMPro.TMP_Text input)
    {
        CommandResponse response = DeveloperConsole.ProcessConsoleInput(input.text);
        inputFeild.text = string.Empty;

        //If the command fails then print out a reason
        if(response.Type == CommandResponse.ResponseType.FAIL)
        {
            AddOutputLog(LogType.Log, response.Message);
        }
    }
}
