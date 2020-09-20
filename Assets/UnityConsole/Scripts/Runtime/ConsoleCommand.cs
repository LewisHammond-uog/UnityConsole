using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lewis.DevConsole
{
    public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
    {

        //Let the user choose the command workd
        [SerializeField] private string commandWord = string.Empty;
        public string CommandWord => commandWord;

        //Let the user choose the description
        [SerializeField] [TextArea] private string commandDescription = "No Descripton";
        public string CommandDescription => commandDescription;

        //Abstract Process function, is implemented by child classes
        public abstract CommandResponse Invoke(string[] args);
    }
}
