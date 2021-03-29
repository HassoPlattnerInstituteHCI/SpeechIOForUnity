using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks; //note that you need to include this if you want to use Task.Delay.
using System.Threading;
using SpeechIO;

public class MyCancellableSpeechOutScript : MonoBehaviour
{
    SpeechOut speechOut;
    CancellationTokenSource source;
    void Start()
    {
        source = new CancellationTokenSource();
        CancellationToken token = source.Token;
        speechOut = new SpeechOut();
        Dialog(token);
    }

    public async void Dialog(CancellationToken token)
    {
        await speechOut.Speak("This is a cancellable speech output script.", token: token);
        await speechOut.Speak("Press space at any time to stop speech out put", token: token);
        await speechOut.Speak("You can add a cancellation token to any speech output", token: token);
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