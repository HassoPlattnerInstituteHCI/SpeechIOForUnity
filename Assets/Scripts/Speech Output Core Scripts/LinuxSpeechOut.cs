using System.Threading.Tasks;
using System.Text.RegularExpressions;
using UnityEngine;

public class LinuxSpeechOut : SpeechBase
{
    public int outputchannel;
    System.Diagnostics.Process speechProcess;
    public override void Init(int outputchannel)
    {
        this.outputchannel = outputchannel;
    }
    public override void Stop()
    {
        if (speechProcess != null)
            if (!speechProcess.HasExited)
                speechProcess.Kill();
    }
    public override async void Speak(string text)
    {
        Debug.Log("[Linux] Speaking: " + text);
        SpeechBase.isSpeaking = true;
        speechProcess = System.Diagnostics.Process.Start("/usr/bin/espeak", "\"" + text + "\"");
        while (!speechProcess.HasExited)    // now wait until finished speaking
        {
            await Task.Delay(100);
        }
        if (speechProcess.ExitCode == 1)
        {
            throw new System.OperationCanceledException("espeak process terminated with exit code -1.");
        }
        SpeechBase.isSpeaking = false;
    }
}
