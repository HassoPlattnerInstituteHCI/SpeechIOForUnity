//==========================================
// Title:  LanguageSwitchScript.cs
// Author: Jotaro Shigeyama (jotaro.shigeyama [at] hpi.de)
// Date:   2020.04.23
//==========================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
public class LanguageSwitchScript : MonoBehaviour
{
    SpeechOut speechOut;
    void Start()
    {
        speechOut = new SpeechOut();
        Dialog();
    }

    /*
        Note:    In order to run this script you need to install all language from OS settings.
        Hinweis: Um dieses Program auszuführen, müssen Sie alle unten Sprachmodul installieren.
        注意：    予めOSの設定からそれぞれの合成音声モジュールをインストールしてください。
    */

    async void Dialog(){
        speechOut.SetLanguage(SpeechBase.LANGUAGE.ENGLISH);
        await speechOut.Speak("The best way to predict the future is to invent it.");
        speechOut.SetLanguage(SpeechBase.LANGUAGE.GERMAN);
        await speechOut.Speak("Die beste Methode die Zukunft vorherzusagen besteht darin, sie zu erfinden");
        speechOut.SetLanguage(SpeechBase.LANGUAGE.JAPANESE);
        await speechOut.Speak("未来を予測する最も良い方法は、それを発明することだ。");
    }

    void OnApplicationQuit(){
        speechOut.Stop(); //Windows: do not remove this line.
    }
}
