//==========================================
// Title:  VoiceCommandBase.cs
// Author: Jotaro Shigeyama (jotaro.shigeyama [at] hpi.de)
// Date:   2020.04.20
//==========================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VoiceCommandBase
{
    public delegate void OnRecognized(string command);
    public static OnRecognized onRecognized;
    public static string[] commands;
    public static bool _isRecognized = false;
    public static string recognized = "";
    public VoiceCommandBase(OnRecognized onRecognized)
    {
        VoiceCommandBase.onRecognized = onRecognized;
    }
    public VoiceCommandBase(OnRecognized onRecognized, string[] commands)
    {
        VoiceCommandBase.onRecognized = onRecognized;
        VoiceCommandBase.commands = commands;
    }
    public static bool isRecognized() { return _isRecognized; }
    public static void setRecognized(bool set) { _isRecognized = set; }
    public abstract void StartListening(); //activates voice recognition without specifying commands.
    public abstract void StartListening(string[] commands); //activates voice recognition with specifying commands
    public abstract void PauseListening(); //pause voice recognition while keep process running.
    public abstract void StopListening(); //terminates voice recognition. kills process.
    public abstract void KeyTriggered(string command);
}