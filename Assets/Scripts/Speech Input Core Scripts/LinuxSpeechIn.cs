using UnityEngine;

public class LinuxSpeechIn : VoiceCommandBase
{
    public LinuxSpeechIn(OnRecognized onRecognized) : base(onRecognized)
    {
        VoiceCommandBase.onRecognized = onRecognized;
        VoiceCommandBase.commands = new string[] { }; //default
    }

    public LinuxSpeechIn(OnRecognized onRecognized, string[] commands) : base(onRecognized, commands)
    {
        VoiceCommandBase.onRecognized = onRecognized;
        VoiceCommandBase.commands = commands;
    }

    public override void KeyTriggered(string message)
    {
        Debug.Log("[keytrigger]:" + message);
        VoiceCommandBase.onRecognized(message);
        VoiceCommandBase.setRecognized(true);
        VoiceCommandBase.recognized = message;
    }

    public override void PauseListening()
    {
    }

    public override void StartListening()
    {
    }

    public override void StartListening(string[] commands)
    {
    }

    public override void StopListening()
    {
    }
}