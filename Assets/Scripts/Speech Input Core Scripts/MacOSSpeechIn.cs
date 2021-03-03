//==========================================
// Title:  MacOSSpeechIn.cs
// Author: Jotaro Shigeyama (jotaro.shigeyama [at] hpi.de)
// Date:   2020.04.20
//==========================================

using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

public class MacOSSpeechIn : VoiceCommandBase
{
  private Thread _thread;
  private static bool isRunning = false;
  public MacOSSpeechIn(OnRecognized　onRecognized):base(VoiceCommandBase.onRecognized){
      VoiceCommandBase.onRecognized = onRecognized;
      VoiceCommandBase.commands = new string[]{}; //default
  }
  public MacOSSpeechIn(OnRecognized　onRecognized, string[] commands):base(VoiceCommandBase.onRecognized,commands){
      VoiceCommandBase.onRecognized = onRecognized;
      VoiceCommandBase.commands = commands;
  }

  private void ThreadRun(){
    // initialize NSSpeech on macOS, then set the command dictionary
    InitLogCallback();
  }

  public delegate void logCallback(string message);

  [AOT.MonoPInvokeCallback(typeof(logCallback))]

  public static void LogReceived (string message) {
    Debug.Log("[Native]:"+message);
    VoiceCommandBase.onRecognized(message);
    VoiceCommandBase.setRecognized(true);
    VoiceCommandBase.recognized = message;
  }

  [DllImport("NSSpeechForUnity")]
  private static extern void _initLogCallback(logCallback callback);
  [DllImport("NSSpeechForUnity")]
  private static extern void  _startDictation();
  [DllImport("NSSpeechForUnity")]
  private static extern void _endDictation();
  [DllImport("NSSpeechForUnity")]
  private static extern void _addCommand(string command);
  [DllImport("NSSpeechForUnity")]
  private static extern void _clearCommand();

  public static void InitLogCallback() {
    if (Application.platform == RuntimePlatform.OSXEditor) {
        _initLogCallback(LogReceived);
    }
    setCommands(VoiceCommandBase.commands);
    _startDictation();
  }
  public static void setCommands(string[] _commands){
    _clearCommand();
    foreach(string command in _commands){
      _addCommand(command);
    }
  }
  public override void StartListening(){
    isRunning = true;
    // run a thread to keep dictation loop
    _thread = new Thread(ThreadRun);
    _thread.IsBackground = true;
    _thread.Start();
  }
  public override void StartListening(string[] commands){
    // run a thread to keep dictation loop
    if(isRunning){
      setCommands(commands);
      return;
    }
    StartListening();
    VoiceCommandBase.commands = commands;
  }
  public override void PauseListening(){
    _clearCommand(); //mac speech will pause listening by clearing commands.
  }
  public override void StopListening(){
      _endDictation();
  }
  public override void KeyTriggered(string message)
  {
        Debug.Log("[keytrigger]:" + message);
        VoiceCommandBase.onRecognized(message);
        VoiceCommandBase.setRecognized(true);
        VoiceCommandBase.recognized = message;
    }
}
