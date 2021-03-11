using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks; //note that you need to include this if you want to use Task.Delay.
using System.Threading;
using SpeechIO;

public class MySpeechOutScript : MonoBehaviour
{
    SpeechOut speechOut;
    CancellationTokenSource source;
    void Start()
    {
        // Addind a CancellationToken is only necessary if you wish to stop speechout
        source = new CancellationTokenSource();
        CancellationToken token = source.Token;
        speechOut = new SpeechOut();
        Dialog(token);
    }

    public async void Dialog(CancellationToken token)
    {
        await speechOut.Speak("This is a sample speech output script.", token: token);
        await speechOut.Speak("If you hear my voice, the speech out system is functional.", token: token);
        await speechOut.Speak("You may hear me in a different accent. If that's the case, please check your OS speech settings.", token: token);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            source.Cancel();
        }
    }

    void OnApplicationQuit()
    {
        speechOut.Stop(); //Windows: do not remove this line.
    }
}