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
        List<string> metaCommands = new List<string> { "repeat", "quit", "options" };
        string[] activeCommands;
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
            }
        }
        public List<string> GetMetaCommands()
        {
            return metaCommands;
        }
        public string[] GetActiveCommands()
        {
            return activeCommands;
        }
        public void SetMetaCommands(List<string> commands)
        {
            metaCommands = commands;
        }
        public string[] AppendMetaCommands(string[] commands)
        {
            return commands.Concat(metaCommands.ToArray()).ToArray();
        }
        public void StartListening()
        {
            recognizer.StartListening();
        }
        public void StartListening(string[] commands)
        {
            recognizer.StartListening(AppendMetaCommands(commands));
        }
        public async Task<string> Listen(Dictionary<string, KeyCode> dict)
        {
            activeCommands = dict.Keys.ToArray<string>();
            recognizer.StartListening(AppendMetaCommands(activeCommands));
            while (!VoiceCommandBase.isRecognized())
            {
                foreach (KeyValuePair<string, KeyCode> command in dict) {
                    if (Input.GetKey(command.Value)) {
                        recognizer.KeyTriggered(command.Key);
                    }
                }
                await Task.Delay(100);
            }
            recognizer.PauseListening();
            VoiceCommandBase.setRecognized(false);
            return VoiceCommandBase.recognized;
        }
        public async Task<string> Listen(string[] commands, bool shortkey = true)
        {
            activeCommands = commands;
            recognizer.StartListening(AppendMetaCommands(commands));
            while (!VoiceCommandBase.isRecognized())
            {
                if (shortkey) {
                    if (Input.GetKey("1"))
                        recognizer.KeyTriggered(commands[0]);
                    else if (Input.GetKey("2") && commands.Length>1)
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
        public void PauseListening()
        {
            recognizer.PauseListening();
        }
        public void StopListening()
        {
            recognizer.StopListening();
        }
    }
}