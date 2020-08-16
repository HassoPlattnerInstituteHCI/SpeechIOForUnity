//
//  native.h
//  NativePlugin
//
//  Created by  Jotaro Shigeyama on 16.04.20.
//  Copyright © 2020  Jotaro Shigeyama. All rights reserved.
//

#ifndef native_h
#define native_h


typedef void (*logCallbackFunc)(const char *);
static logCallbackFunc logCallback;

#endif /* native_h */
