using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;
public class SpeechSpeedScript : MonoBehaviour
{
    SpeechOut speechOut = new SpeechOut();
    // Start is called before the first frame update
    void Start()
    {
        Dialog();
    }
    async void Dialog(){
        await speechOut.Speak("Patty cake, patty cake, baker's man.", 1.0f);
        await speechOut.Speak("Bake me a cake as fast as you can.",   1.3f);
        await speechOut.Speak("Roll it, pat it, mark it with a B.",   1.6f);
        await speechOut.Speak("Put it on the oven for baby and me.",  2.0f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnApplicationQuit(){
        speechOut.Stop(); //Windows: do not remove this line.
    }
}
