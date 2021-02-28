public class LinuxSpeechIn : VoiceCommandBase
{
    public LinuxSpeechIn(OnRecognized onRecognized) : base(onRecognized)
    {
    }

    public LinuxSpeechIn(OnRecognized onRecognized, string[] commands) : base(onRecognized, commands)
    {
    }

    public override void KeyTriggered(string command)
    {
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