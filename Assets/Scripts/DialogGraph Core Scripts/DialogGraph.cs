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
        private List<DialogOption> options;   // the options to move on to other nodes
        private string sentences;             // message to speak when node is active
        private DialogAction action;          // action to perform when node is triggered
        private object actionarg;
        private AudioSource soundSource;      // if playing sounds, a soundSource needs to be specified
        private SpeechIn asr;                 // the speech recognition object from your app
        private SpeechOut tts;                // the speech synthesis object from your app
        /**
         * constructor
         * @param sentences - string with spoken sentences to speak when the node is active
         * @param callBack - optional callback method in your app that should be called when the node is active
         * @param param - parameters for the callback method
         * override for playing audio files
         * @param source - AudioSource in your Unity3D project that plays the file
         * @param playSound - AudioClip which has to be played when the node is active
         */
        public DialogNode(string sentences, DialogAction callback = null, object param = null)
        {
            this.sentences = sentences;
            options = new List<DialogOption>();
            action = callback;
            actionarg = param;
        }
        public DialogNode(AudioSource source, AudioClip playSound, DialogAction callback = null, object param = null)
        {
            sentences = "";
            soundSource = source;
            soundSource.clip = playSound;
            options = new List<DialogOption>();
            action = callback;
            actionarg = param;
        }
        /**
         * adds an option (edge) to the node that lead to activation of other nodes or events once this node is deactivated
         * @param node - if only a node is provided, the node will automatically trigger that after it is deactivated
         * override AddOption(string command, DialogNode node)
         * @param command - command that should be spoken to trigger the specified node
         * override AddOption(List<string> commands, DialogNode node)
         * param commands - various commands that can be used to trigger the specified node (for example "yes","yea","ok")
         */
        internal void AddOption(DialogNode node)
        {
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
        /**
         * efficiency method to add up to three options in one call
         * and its override to do the same thing with single string triggers
         */
        internal void AddOptions(List<string> commands1 = null, DialogNode node1 = null,
                                    List<string> commands2 = null, DialogNode node2 = null,
                                    List<string> commands3 = null, DialogNode node3 = null)
        {
            AddOption(commands1, node1);
            if (commands2 != null)
                AddOption(commands2, node2);
            if (commands3 != null)
                AddOption(commands3, node3);
        }
        internal void AddOptions(string command1 = null, DialogNode node1 = null,
                                    string command2 = null, DialogNode node2 = null,
                                    string command3 = null, DialogNode node3 = null)
        {
            AddOption(command1, node1);
            if (command2 != null)
                AddOption(command2, node2);
            if (command3 != null)
                AddOption(command3, node3);
        }
        /**
         * plays a sound that was pre-defined when the node was created
         */
        internal async Task playSound()
        {
            soundSource.Play();
            while (soundSource.isPlaying)
                await Task.Delay(100);
            return;
        }
        /**
         * the routine to activate the node. When called externally this triggers the entire graph
         * and will continue until a leaf node is reached
         * @param speechIn - the SpeechIn object defined in your app. Avoid having multiple instances of this!
         * @param speechout - the SpeechOut object defined in your app. Avoid having multiple instances of this!
         * @param silent - optional parameter which defaults to false. If true it uses the node as a vehicle to progress in the graph without speaking or making any sound
         */
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
        /**
         * internal helper method that populates a string array with all commands that are specified by the options
         */
        internal string[] GenerateCommandArray()
        {
            List<string> commandList = new List<string>();
            foreach (DialogOption option in options)
                commandList.AddRange(option.commands);
            return commandList.ToArray();
        }
        /**
         * if meta commands are defined, they should be handled appropriately
         */
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
        public List<string> commands;   // the spoken command to traverse this edge in the graph
        public DialogNode next;         // the node that should be triggered when this edge is traversed
        public DialogOption(DialogNode next, List<string> commands = null)
        {
            this.commands = commands;
            this.next = next;
        }
    }
}
