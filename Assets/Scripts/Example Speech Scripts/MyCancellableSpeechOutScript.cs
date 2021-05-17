using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks; //note that you need to include this if you want to use Task.Delay.
using System.Threading;
using SpeechIO;

public class MyCancellableSpeechOutScript : MonoBehaviour
{
    SpeechOut speechOut;
    void Start()
    {
        speechOut = new SpeechOut();
        Dialog();
    }

    public async void Dialog()
    {
        await speechOut.Speak("This is a cancellable speech output script.");
        await speechOut.Speak("Press space at any time to stop speech out put");
        await speechOut.Speak("You can add a cancellation token to any speech output");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            speechOut.Stop();
        }
    }

    void OnApplicationQuit()
    {
        speechOut.Stop(); //Windows: do not remove this line.
    }
}