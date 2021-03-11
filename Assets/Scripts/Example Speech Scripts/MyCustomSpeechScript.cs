using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpeechIO;

public class MyCustomSpeechScript : MonoBehaviour
{
    public string[] commands = new string[] { "sushi", "sashimi", "ramen" };
    SpeechIn speechIn;
    void Start()
    {
        Debug.Log("Hit spacebar to pause, hit return to change commands");
        speechIn = new SpeechIn(onRecognized, commands);
        speechIn.StartListening(new string[] { "hungry" }); // you can override here
    }
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            speechIn.PauseListening(); //pause listening ([mac] clears all commands)
        }
        if (Input.GetKeyDown("return"))
        {
            speechIn.StartListening(commands); //you can update commands
        }
    }
    void onRecognized(string message)
    {
        Debug.Log("[MyScript]: " + message);
    }
    public void OnApplicationQuit()
    {
        speechIn.StopListening(); // [mac] do not delete this line
    }
}
