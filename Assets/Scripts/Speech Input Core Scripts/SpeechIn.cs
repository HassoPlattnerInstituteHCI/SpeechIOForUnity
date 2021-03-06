//==========================================
// Title:  SpeechIn.cs
// Author: Jotaro Shigeyama and Thijs Roumen (firstname.lastname [at] hpi.de)
// Date:   2020.05.16
//==========================================

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeechIO
{
    public class SpeechIn
    {
        VoiceCommandBase recognizer;
        List<string> metaCommands = new List<string> { "repeat", "quit", "options" }; //sets default meta-commands the SpeechIn should always listen to
        string[] activeCommands;
        /**
         * constructor for SpeechIn
         * constructs the correct sub-implementation depending on your OS (Windows or macOS)
         * @param onRecognized - callback function to implement in your app that gets triggered when speech is recognized
         *        (you can leave the method empty or specify behavior for meta-commands there)
         * override method constucts SpeechIn with pre-defined commands to listen to when startListening is used
         * @param commands - array of strings with words for speech recognition to listen to
         */
        public SpeechIn(VoiceCommandBase.OnRecognized onRecognized)
        {
            if (Application.platform == RuntimePlatform.OSXEditor ||
               Application.platform == RuntimePlatform.OSXPlayer)
            {
                recognizer = new MacOSSpeechIn(onRecognized);
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer)
            {
                recognizer = new WindowsSpeechIn(onRecognized);
            } else if (Application.platform == RuntimePlatform.LinuxEditor ||
                    Application.platform == RuntimePlatform.LinuxPlayer)
            {
                recognizer = new LinuxSpeechIn(onRecognized);
            }
        }
        public SpeechIn(VoiceCommandBase.OnRecognized onRecognized, string[] commands)
        {
            if (Application.platform == RuntimePlatform.OSXEditor ||
               Application.platform == RuntimePlatform.OSXPlayer)
            {
                recognizer = new MacOSSpeechIn(onRecognized, commands);
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer)
            {
                recognizer = new WindowsSpeechIn(onRecognized, commands);
            } else if (Application.platform == RuntimePlatform.LinuxEditor ||
                    Application.platform == RuntimePlatform.LinuxPlayer)
            {
                recognizer = new LinuxSpeechIn(onRecognized, commands);
            }
        }
        public string[] GetActiveCommands()
        {
            return activeCommands;
        }
        /**
         * Sets the meta commands, these are commands the SpeechIn should always listen to,
         * declaring them as meta-commands is mostly for convenience and to not forget some basic metaCommands
         * default examples are "quit, repeat, options", but you could have more app-specific commands
         * @param commands - a list of strings speechIn should always listen to
         */
        public void SetMetaCommands(List<string> commands)
        {
            metaCommands = commands;
        }
        public List<string> GetMetaCommands()
        {
            return metaCommands;
        }
        public string[] AppendMetaCommands(string[] commands)
        {
            return commands.Concat(metaCommands.ToArray()).ToArray();
        }
        /**
         * starts listening to all previously set commands and meta-commands
         * (and its override function with additional commands as params)
         * if possible, we recommend using the asynchronous Listen() method below instead
         */
        public void StartListening()
        {
            recognizer.StartListening();
        }
        public void StartListening(string[] commands)
        {
            recognizer.StartListening(AppendMetaCommands(commands));
        }
        /**
         * asynchronous task to listen for specific commands
         * @param commands - an array of strings with the commands
         * @param shortkey - optional parameter defaults to true which uses keypress 1,2,3 to override the first three commands if not recognized properly
         * override function Listen (Dictionary<string, KeyCode> dict)
         *  @param dict - a dictionary of commands and keycodes to specify what keycode overrides which command
         */
        public async Task<string> Listen(string[] commands, bool shortkey = true)
        {
            activeCommands = commands;
            recognizer.StartListening(AppendMetaCommands(commands));
            while (!VoiceCommandBase.isRecognized())
            {
                if (shortkey)
                {
                    if (Input.GetKey("1"))
                        recognizer.KeyTriggered(commands[0]);
                    else if (Input.GetKey("2") && commands.Length > 1)
                        recognizer.KeyTriggered(commands[1]);
                    else if (Input.GetKey("3") && commands.Length > 2)
                        recognizer.KeyTriggered(commands[2]);
                }
                await Task.Delay(100);
            }
            recognizer.PauseListening();
            VoiceCommandBase.setRecognized(false);
            return VoiceCommandBase.recognized;
        }
        public async Task<string> Listen(Dictionary<string, KeyCode> dict)
        {
            activeCommands = dict.Keys.ToArray<string>();
            recognizer.StartListening(AppendMetaCommands(activeCommands));
            while (!VoiceCommandBase.isRecognized())
            {
                foreach (KeyValuePair<string, KeyCode> command in dict)
                {
                    if (Input.GetKey(command.Value))
                    {
                        recognizer.KeyTriggered(command.Key);
                    }
                }
                await Task.Delay(100);
            }
            recognizer.PauseListening();
            VoiceCommandBase.setRecognized(false);
            return VoiceCommandBase.recognized;
        }
        /**
         * pause the speech recognition, pre-set commands will remain active
         */
        public void PauseListening()
        {
            recognizer.PauseListening();
        }
        /**
         * stop the speech recognition, all commands other than metaCommands are
         * lost as well as the speechIn instance on your OS
         * its strongly recommended to do this before closing down your application
         */
        public void StopListening()
        {
            recognizer.StopListening();
        }
    }
}
