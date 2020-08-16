import Foundation  
import AppKit  
class Dispatcher:  NSObject, NSSpeechRecognizerDelegate {
    var stop: Bool  
    override init () {stop = false}  
    func speechRecognizer(_ sender: NSSpeechRecognizer, didRecognizeCommand command: String)
    {
        print("command: \(command)")
    }
}

let dispatcher = Dispatcher()
let recognizer = NSSpeechRecognizer()!

recognizer.delegate = dispatcher
recognizer.commands = ["apple","orange","banana"]
recognizer.startListening()
recognizer.displayedCommandsTitle = "test"

let loop = RunLoop.current
let mode = loop.currentMode ?? RunLoop.Mode.default
while loop.run(mode:mode, before: Date(timeIntervalSinceNow: 0.1))
      && !dispatcher.stop {}