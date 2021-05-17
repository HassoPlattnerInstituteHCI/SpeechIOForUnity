using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class WindowsSpeechOut : SpeechBase
{
    [DllImport("WindowsVoice")]
    public static extern void initSpeech();
    [DllImport("WindowsVoice")]
    public static extern void destroySpeech();
    [DllImport("WindowsVoice")]
    public static extern void addToSpeechQueue(string s);
    [DllImport("WindowsVoice")]
    public static extern void clearSpeechQueue();
    [DllImport("WindowsVoice")]
    public static extern void statusMessage(StringBuilder str, int length);
    public static WindowsSpeechOut theVoice = null;
    // Use this for initialization
    public bool IsSpeaking()
    {
        string message = GetStatusMessage();
        if (message == "Waiting." || message=="Speech destroyed.") return false;
        else return true;
    }
    public static void _speak(string msg, float delay = 0f)
    {

        if (delay == 0f)
            addToSpeechQueue(msg);
    }
    public override async void Speak(string text)
    {
        if (text == "" || text == " ") return;
        Debug.Log("[WinSpeech]:" + text);
        text = "<voice required=\"Language=" + GetLangID(Language) + "\">" + text + "</voice>";
        _speak(text);
        SpeechBase.isSpeaking = true;
        while (GetStatusMessage() == "Waiting.")
        {
            await Task.Delay(100);
        }
        while (IsSpeaking())
        {
            await Task.Delay(100);
        }
        SpeechBase.isSpeaking = false;
        return;
    }
    public override void Init(int outputchannel)
    {
        if (theVoice == null)
        {
            theVoice = this;
            initSpeech();
            Debug.Log("[WinSpeech]:Initialized");
        }
    }
    public override void Stop()
    {
        if (theVoice == this)
        {
            Debug.Log("[WinSpeech]:Destroying speech");
            destroySpeech();
            clearSpeechQueue();
            Debug.Log("[WinSpeech]:Speech destroyed");
            theVoice = null;
        }
    }
    public static string GetStatusMessage()
    {
        StringBuilder sb = new StringBuilder(512);
        statusMessage(sb, 512);
        return sb.ToString();
    }
    private string GetLangID(LANGUAGE lang)
    {
        switch (lang)
        {
            case LANGUAGE.ENGLISH:
                return "409";
            case LANGUAGE.DUTCH:
                return "413";
            case LANGUAGE.GERMAN:
                return "407";
            case LANGUAGE.JAPANESE:
                return "411";
            default: //English
                return "409";
        }
    }
}
