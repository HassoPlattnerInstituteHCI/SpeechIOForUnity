//==========================================
// Title:  DialogGraph.cs
// Author: Jotaro Shigeyama and Thijs Roumen (firstname.lastname [at] hpi.de)
// Date:   2020.05.16
//==========================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeechIO
{
    public delegate Task DialogAction(object param);

    public class DialogNode
    {
        private List<DialogOption> options;
        private string sentences;
        private DialogAction action;
        private object actionarg;
        private AudioSource soundSource;
        private SpeechIn asr;
        private SpeechOut tts;
        public DialogNode(string sentences, DialogAction callback = null, object param = null)    {
            this.sentences = sentences;
            options = new List<DialogOption>();
            action = callback;
            actionarg = param;
        }
        public DialogNode(AudioSource source,AudioClip playSound, DialogAction callback = null, object param = null)
        {
            sentences = "";
            soundSource = source;
            soundSource.clip = playSound;
            options = new List<DialogOption>();
            action = callback;
            actionarg = param;
        }
        internal void AddOption(DialogNode node) {
            options.Add(new DialogOption(node));
            }
        internal void AddOption(string command, DialogNode node)
        {
            List<string> commands = new List<string>();
            commands.Add(command);
            options.Add(new DialogOption(node, commands));
        }
        internal void AddOption(List<string> commands, DialogNode node)
        {
            options.Add(new DialogOption(node, commands));
        }
        internal void AddOptions(   List<string> commands1 = null, DialogNode node1 = null,
                                    List<string> commands2 = null, DialogNode node2 = null,
                                    List<string> commands3 = null, DialogNode node3 = null)
        {
            AddOption(commands1, node1);
            if (commands2 != null) 
                AddOption(commands2, node2);
            if (commands3 != null)
                AddOption(commands3, node3);
        }
        internal void AddOptions(   string command1 = null, DialogNode node1 = null,
                                    string command2 = null, DialogNode node2 = null,
                                    string command3 = null, DialogNode node3 = null)
        {
            AddOption(command1, node1);
            if (command2 != null)
                AddOption(command2, node2);
            if (command3 != null)
                AddOption(command3, node3);
        }
        internal async Task playSound() {
            soundSource.Play();
            while (soundSource.isPlaying)
                await Task.Delay(100);
            return;
        }
        public async void Play(SpeechIn speechIn, SpeechOut speechOut, bool silent = false)
        {
            asr = speechIn;
            tts = speechOut;
            if (!silent & sentences != "") await tts.Speak(sentences);
            if (soundSource && soundSource.clip)
                await playSound();
            if (action != null) await action.Invoke(actionarg);
            string recognized;
            switch (options.Count)
            {
                case 0: //no options, endNode   
                    return;
                case 1: //single option, listen without return
                    DialogOption singleOption = options.ToArray()[0];
                    if (singleOption.commands == null)  //option has no commands, move on to next node
                    {
                        singleOption.next.Play(asr, tts);
                        return;
                    }
                    recognized = await asr.Listen(singleOption.commands.ToArray());
                    if (CheckMetaCommands(recognized) == false)
                        singleOption.next.Play(asr, tts);
                    break;
                default: // various options
                    recognized = await asr.Listen(GenerateCommandArray());
                    foreach (DialogOption option in options)
                        if (option.commands.Contains(recognized))
                            option.next.Play(asr, tts);
                    CheckMetaCommands(recognized);
                    return;
            }
        }
        internal string[] GenerateCommandArray()
        {
            List<string> commandList = new List<string>();
            foreach (DialogOption option in options)
                commandList.AddRange(option.commands);
            return commandList.ToArray();
        }
        internal bool CheckMetaCommands(string input)
        {
            if (asr.GetMetaCommands().Contains(input))
            {
                Play(asr, tts, true);
                return true;
            }
            return false;
        }
    }
    public class DialogOption
    {
        public List<string> commands;
        public DialogNode next;
        public DialogOption(DialogNode next, List<string> commands = null)
        {
            this.commands = commands;
            this.next = next;
        }
    }
}