//==========================================
// Title:  SpeechOut.cs
// Author: Jotaro Shigeyama and Thijs Roumen (firstname.lastname [at] hpi.de)
// Date:   2020.05.16
//==========================================

using UnityEngine;
using System.Threading.Tasks;

namespace SpeechIO
{
    public class SpeechOut
    {
        SpeechBase speech;
        string lastSpoken;
        // Use this for initialization
        // outputchannel = optional parameter to select a specific channel, -1 takes your OSs default
        public SpeechOut(int outputchannel = -1) 
        {
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                speech = new MacOSSPeechOut();
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                speech = new WindowsSpeechOut();
            }
            Init(outputchannel);
        }
        public void Init(int outputchannel)
        {
            speech.Init(outputchannel);
        }
        public void Stop()
        {
            speech.Stop();
        }
        public async Task Speak(string text)
        {
            speech.Speak(text);
            while (SpeechBase.isSpeaking)    // now wait until finished speaking
            {
                await Task.Delay(100);
            }
            lastSpoken = text;
            return;
        }
        public async Task Speak(string text, float speed = 1.0f, SpeechBase.LANGUAGE lang = SpeechBase.LANGUAGE.ENGLISH)
        {
            SetSpeed(speed);
            SetLanguage(lang);
            await Speak(text);
        }
        public async Task Repeat()
        {
            await Speak(lastSpoken);
        }
        public void SetLanguage(SpeechBase.LANGUAGE lang)
        {
            SpeechBase.Language = lang;
        }
        public void SetSpeed(float s)
        {
            SpeechBase.speed = s;
        }
    }
}