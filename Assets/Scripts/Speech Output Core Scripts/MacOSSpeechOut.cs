using System.Threading.Tasks;
using System.Text.RegularExpressions;
using UnityEngine;
public class MacOSSPeechOut : SpeechBase
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
        string cmdArgs;
        string voice = GetLangVoice();
        Debug.Log("[Mac] Speaking: " + text);
        int rate = (int)(speed * 170); //[mac]default say : 170 wpm.
        if (text == "crazylaugh")
        {
            cmdArgs = string.Format(" -v Hysterical \"muhahahaha\" ");     //couldnt help myself ;)
        }
        else
        {
            if (outputchannel == -1)
                cmdArgs = string.Format("-r {0} -v {1} \"{2}\" ", rate, voice, text.Replace("\"", ","));
            else
                cmdArgs = string.Format("-r {0} -a {1} -v {2} \"{3}\" ", rate, outputchannel, voice, text.Replace("\"", ","));
        }
        speechProcess = System.Diagnostics.Process.Start("/usr/bin/say", cmdArgs);
        SpeechBase.isSpeaking = true;
        while (!speechProcess.HasExited)    // now wait until finished speaking
        {
            await Task.Delay(100);
        }
        if (speechProcess.ExitCode == 1)
        {
            throw new System.OperationCanceledException("say process terminated with exit code -1. debug: is say in /usr/bin/? (open terminal: which say) and does the selected outputchannel work (open terminal: say -a <outputchannel> testing)");
        }
        SpeechBase.isSpeaking = false;
        return;
    }
    private string GetLangVoice()
    {
        switch (Language)
        {
            case LANGUAGE.DUTCH:
                return "Xander";
            case LANGUAGE.ENGLISH:
                return "Samantha";
            case LANGUAGE.GERMAN:
                return "Anna";
            case LANGUAGE.JAPANESE:
                return "Kyoko"; //京子さん
            default: throw new System.NotSupportedException("your system language is currently not supported, specify your system's voice name in the speechOut construction e.g. speechOut(voicename)");
        }
    }
}
