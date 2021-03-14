using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks; //note that you need to include this if you want to use Task.Delay.
using SpeechIO;

public class MySpeechOutScript : MonoBehaviour
{
    // Start is called before the first frame update
    SpeechOut speechOut;
    void Start()
    {
        speechOut = new SpeechOut();
        Dialog();
    }

    public async void Dialog()
    {
        await speechOut.Speak("This is a sample speech output script.");
        await speechOut.Speak("If you hear my voice, the speech out system is functional.");
        await speechOut.Speak("You may hear me in a different accent. If that's the case, please check your OS speech settings.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnApplicationQuit()
    {
        speechOut.Stop(); //Windows: do not remove this line.
    }

}