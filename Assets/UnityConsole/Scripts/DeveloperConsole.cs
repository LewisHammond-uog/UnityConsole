using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lewis.DevConsole {

    /// <summary>
    /// Background of the developer console, deals with processing and invoking commands
    /// </summary>
    public class DeveloperConsole
    {

        //Prefix that must be before the command for it to be run
        private readonly string commandPrefix;
        //Dictonary of commands <commandWord, command>
        private Dictionary<string, IConsoleCommand> commandsDictonary;

        //Seperator used to seperate commands and arguments
        private const char commandDelimiter = ' ';

        //Constructor for console, takes the prefix for commands and a list/array of commands
        public DeveloperConsole(string prefix, IEnumerable<IConsoleCommand> commands)
        {
            //Set prefix
            commandPrefix = prefix;

            //Check that commands list is not null
            if(commands == null)
            {
                return;
            }

            //Take all of the commands and add them to the dictonary for fast lookup,
            //set the dictonary to 
            commandsDictonary = new Dictionary<string, IConsoleCommand>(StringComparer.OrdinalIgnoreCase);
            foreach(var command in commands)
            {
                commandsDictonary.Add(command.CommandWord, command);
            }
        }

        /// <summary>
        /// Gets a list of all the commands
        /// </summary>
        public IEnumerable GetAllCommands()
        {
            //Return Null if the commands dictonary is not initalised
            if(commandsDictonary == null)
            {
                return null;
            }

            //Return a flattened dictonary
            return commandsDictonary.Values.ToArray();
        }

        /// <summary>
        /// Processes and input from the console
        /// Strips command prefix and splits the string in to command word and arguments
        /// Then Invokes the command actions
        /// does not check if command is valid, this is done in the invoke command function
        /// </summary>
        public void ProcessConsoleInput(string input)
        {
            //Check that command starts with our prefix, otherwise it is invalid
            if (!input.StartsWith(commandPrefix))
            {
                return;
            }
            //Remove the prefix from the string
            input = input.Remove(0, commandPrefix.Length);

            //Split the string to an array
            string[] inputSplit = input.Split(commandDelimiter);

            //Get the command word (the 0th value in the array) and then the
            //arguments which is every word after that
            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            //Actually invoke the command
            InvokeCommand(commandInput, args);
        }

        /// <summary>
        /// Checks if a command exists in the command dictonary
        /// </summary>
        /// <returns></returns>
        public bool CommandExists(string command) {
            return commandsDictonary.ContainsKey(command);
        }

        /// <summary>
        /// Process a command, invoking whatever it is supposed to do
        /// </summary>
        /// <param name="commandInput">Command Input (minus the command prefix)</param>
        /// <param name="args">Command Arguments</param>
        private void InvokeCommand(string commandInput, string[] args)
        {
            //Check that command exists, dictonary ignores cases as it was set in the constructor
            if(!CommandExists(commandInput)){
                Debug.LogError($"Command {commandInput} does not exist");
                return;
            }

            //Invoke the command
            commandsDictonary[commandInput].Invoke(args);
        }

    }

}