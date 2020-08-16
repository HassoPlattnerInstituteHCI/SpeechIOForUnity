//
//  native.m
//  NativePlugin
//
//  Created by  Jotaro Shigeyama on 16.04.20.
//  Copyright © 2020  Jotaro Shigeyama. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>

#include "native.h"

void sendLog(const char * l){
    logCallback(l);
}
@interface RecognizerDelegate : NSObject <NSSpeechRecognizerDelegate>
@property (nonatomic) NSSpeechRecognizer *recognizer;
@property (nonatomic) BOOL didRecognize;
@property (nonatomic) BOOL didEnd;
@end

@implementation RecognizerDelegate
- (id)init{
    if ((self = [super init])) {
        self.didRecognize = NO;
        self.didEnd = NO;
        self.recognizer = [[NSSpeechRecognizer alloc] init];
        self.recognizer.delegate = self;
        self.recognizer.commands = @[];
        [self.recognizer startListening];
        self.recognizer.displayedCommandsTitle=@"unity";
    }
    return self;
}
- (void)speechRecognizer:(NSSpeechRecognizer *)sender didRecognizeCommand:(NSString *)command
{
    if(logCallback != NULL){
        sendLog([command UTF8String]);
    }
}

- (void)addCommand:(NSString *) string
{
    NSMutableArray<NSString *> *array = [self.recognizer.commands mutableCopy];
    [array addObject:string];
    self.recognizer.commands = array;
}

- (void)clearCommand
{
    NSArray<NSString *> *array = @[];
    self.recognizer.commands = array;
}

@end

RecognizerDelegate *recognizerDelegate ;
void _initLogCallback(logCallbackFunc callback) {
    recognizerDelegate = [[RecognizerDelegate alloc] init];
    logCallback = callback;
    if(logCallback != NULL && recognizerDelegate.didEnd == NO){
        // sendLog([@"Mac: Speech is ready" UTF8String]);
    }
}
void _startDictation(){
    // sendLog([@"Mac: Speech is started" UTF8String]);
    while (recognizerDelegate.didEnd == NO) {
        
        [[NSRunLoop currentRunLoop] runUntilDate:[NSDate dateWithTimeIntervalSinceNow:1.0]];
    }
    // sendLog([@"Mac: Speech is ended" UTF8String]);
    [recognizerDelegate.recognizer stopListening];
}
void _endDictation(){
    recognizerDelegate.didEnd = YES;
}
void _addCommand(const char *string){
    [recognizerDelegate addCommand: [NSString stringWithUTF8String:string]];
}

void _clearCommand(){
    [recognizerDelegate clearCommand];
}

