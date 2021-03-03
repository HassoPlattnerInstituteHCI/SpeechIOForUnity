using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using SpeechIO;

public class MySpeechInScript : MonoBehaviour
{
    SpeechIn speechIn;
    string[] commands = new string[] { "apple", "banana", "orange" };
    Dictionary<string, KeyCode> dict = new Dictionary<string, KeyCode>();
    void Start()
    {
        speechIn = new SpeechIn(onRecognized);
        Dialog();
    }
    async void Dialog()
    {
        dict.Add("apple", KeyCode.A);
        dict.Add("banana", KeyCode.B);
        dict.Add("orange", KeyCode.Space);
        await speechIn.Listen(dict);
    }
    void onRecognized(string message)
    {
        Debug.Log("[MyScript]: " + message);
    }
    public void OnApplicationQuit()
    {
        speechIn.StopListening(); // [macOS] do not delete this line!
    }
}
