using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lewis.DevConsole
{
    /// <summary>
    /// Interface for the layouts of console commands
    /// </summary>
    public interface IConsoleCommand
    {
        string CommandWord { get; }
        string CommandDescription { get; }
        CommandResponse Invoke(string[] args);
    }

    /// <summary>
    /// Class for responses to commands issued the the console
    /// Has a state (pass or fail) and a comment, usally about
    /// why it failed
    /// </summary>
    public class CommandResponse { 
        public enum ResponseType
        {
            FAIL = 0,
            PASS = 1
        }

        public ResponseType type { get; } //Pass or fail
        public string message; //Message associated with the pass or fail

        //Constructor, must define type message can be left for passes
        public CommandResponse(ResponseType type, string message = "")
        {
            //Set Values
            this.type = type;
            this.message = message;

            //Throw a warning if we have a fail and do no provide a reason
            if(this.type == ResponseType.FAIL && message == "")
            {
                Debug.LogWarning("Fail CommandRespose created without a reason");
            }
        }

    }
}
