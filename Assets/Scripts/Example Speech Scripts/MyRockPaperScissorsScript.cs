using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using SpeechIO;

public class MyRockPaperScissorsScript : MonoBehaviour
{
    SpeechIn speechIn;
    SpeechOut speechOut;
    AudioSource source;
    string[] commands = new string[] { "rock", "paper", "scissors" };

    void Start()
    {
        speechIn = new SpeechIn(onRecognized);
        speechOut = new SpeechOut();
        Dialog();
        speechIn.SetMetaCommands(new List<string> { "repeat", "quit", "options" });
    }
    private async void Dialog()
    {

        await speechOut.Speak("Welcome to the Rock Paper Scissors app");

        DialogNode start = new DialogNode("say rock, paper or scissors to begin");
        DialogNode playRock = new DialogNode("", playRPS, 0);
        DialogNode playPaper = new DialogNode("", playRPS, 1);
        DialogNode playScissors = new DialogNode("", playRPS, 2);
        DialogNode replay = new DialogNode("do you dare to play again?");
        DialogNode end = new DialogNode("it was nice playing with you!", quitApplication);
        start.AddOptions("rock", playRock, "scissors", playScissors, "paper", playPaper);
        playRock.AddOption(replay);
        playPaper.AddOption(replay);
        playScissors.AddOption(replay);
        replay.AddOptions("yes", start, "no", end);

        start.Play(speechIn, speechOut);
    }
    private Task quitApplication(object i)
    {
        OnApplicationQuit();
        Application.Quit();
        return Task.CompletedTask;
    }
    private async Task playRPS(object param)
    {
        int pc;
        int choice = (int)param;
        pc = Random.Range(0, 3);
        await speechOut.Speak("Then I choose... " + commands[pc]);
        switch (Judge(choice, pc))
        {
            case 0:
                //draw
                await speechOut.Speak("its a draw!");
                break;
            case 1:
                // me wins
                await speechOut.Speak("dammit... you win!");
                break;
            case 2:
                // pc wins
                await speechOut.Speak("crazylaugh");
                await speechOut.Speak("yea! ... I win!");
                break;
        }
        return;
    }

    async void onRecognized(string message)
    {
        // handle defined meta-commands
        switch (message)
        {
            case "repeat":
                await speechOut.Repeat();
                break;
            case "quit":
                await speechOut.Speak("Thanks for using our application. Closing down now...");
                OnApplicationQuit();
                Application.Quit();
                break;
            case "options":
                string commandlist = "";
                foreach (string command in speechIn.GetActiveCommands())
                {
                    commandlist += command + ", ";
                }
                await speechOut.Speak("currently available commands: " + commandlist);
                break;
        }
    }

    private int Judge(int me, int pc)
    {
        if (me == pc) return 0; //draw
        else if ((me == 0 && pc == 1) || (me == 1 && pc == 2) || (me == 2 && pc == 0)) return 2; //PC win
        else return 1; //me win
    }
    public void OnApplicationQuit()
    {
        speechIn.StopListening(); // [mac] do not delete this line!
        speechOut.Stop(); // [win] do not delete this line!
    }
}
