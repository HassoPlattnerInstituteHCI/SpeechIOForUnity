//==========================================
// Title:  SpeechOut.cs
// Author: Jotaro Shigeyama and Thijs Roumen (firstname.lastname [at] hpi.de)
// Date:   2020.05.16
//==========================================

using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

namespace SpeechIO
{
    public class SpeechOut
    {
        SpeechBase speech;
        CancellationTokenSource source;
        CancellationToken token;
        string lastSpoken;
        /**
         * constructor for SpeechOut
         * @param outputchannel = optional parameter to select a specific channel, -1 takes your OSs default
         */
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
            else if (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.LinuxPlayer) {
                speech = new LinuxSpeechOut();
            }
            Init(outputchannel);
            source = new CancellationTokenSource();
            token = source.Token;
        }
        private void Init(int outputchannel)
        {
            speech.Init(outputchannel);
        }
        /**
         * kills the OS process to send speechOut
         * if using the default stopAll it kills all ongoing speech by calling with stopAll = false it will only kill the last spoken one
         * use this before you close down your application to avoid ghost processes on your OS
         */
        public void Stop(bool stopAll = true)
        {
            if (stopAll)
                source.Cancel();
            else
                speech.Stop();
        }
        /**
         * sends the speech command to your OS' process
         * note that it is asynchronously to ensure it finishes speaking if you call multiple times
         * @param text - the string to be spoken
         * @param speed - optional float to control the speed of speaking (defaults to 1.0)
         * @param lang - optional parameter to control the language of speaking (defaults to english)
         */
        public async Task Speak(string text, float speed = 1.0f, SpeechBase.LANGUAGE lang = SpeechBase.LANGUAGE.ENGLISH)
        {
            SetSpeed(speed);
            SetLanguage(lang);
            speech.Speak(text);
            while (SpeechBase.isSpeaking)    // now wait until finished speaking
            {
                if (token != null && ((CancellationToken)token).IsCancellationRequested)
                {
                    Stop(false);
                    break;
                }
                await Task.Delay(100);
            }
            lastSpoken = text;
            return;
        }
        /**
         * routine to repeat a spoken sentence
         * useful to implement in combination with a metaCommand "repeat" in SpeechIn
         */
        public async Task Repeat()
        {
            await Speak(lastSpoken);
        }
        /**
         * sets the language of the speech
         * @param lang - language enum implemented in speechBase. For non-included languages contact us
         */
        public void SetLanguage(SpeechBase.LANGUAGE lang)
        {
            SpeechBase.Language = lang;
        }
        /**
         * sets the speed of the speech
         * @param speed - float to determine how fast to speak relative to 1.0 (default)
         */
        public void SetSpeed(float speed)
        {
            SpeechBase.speed = speed;
        }
        public bool IsSpeaking()
        {
            return SpeechBase.isSpeaking;
        }
    }
}
