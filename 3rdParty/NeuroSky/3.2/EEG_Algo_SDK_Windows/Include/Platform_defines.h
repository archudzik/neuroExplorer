/**
  ******************************************************************************
  * @file    Platform_Defines.h
  * @author  Algo SDK Team
  * @version V0.1
  * @date    25-August-2015
  * @brief   Data types for Windows
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; COPYRIGHT(c) NeuroSky Inc. All rights reserved.</center></h2>
  *
  *
  ******************************************************************************
  */

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __PLATFORM_DEFINES_H
#define __PLATFORM_DEFINES_H

#include <math.h>

#ifdef __cplusplus
extern "C" {
#endif

// Supported EEG Algos
#define SUPPORT_ALGO_ATT
#define SUPPORT_ALGO_MED
#define SUPPORT_ALGO_BP
#define SUPPORT_EYE_BLINK_DETECTION


#define NSK_ALGO_DECL(ret, func, args)		__declspec(dllexport) ret __cdecl func args

#ifndef NS_INT
typedef int NS_INT;
#endif

#ifndef NS_UINT
typedef unsigned int NS_UINT;
#endif

#ifndef NS_INT8
typedef char NS_INT8;
#endif

#ifndef NS_UINT8
typedef unsigned char NS_UINT8;
#endif

#ifndef NS_INT16
typedef short NS_INT16;
#endif

#ifndef NS_UINT16
typedef unsigned short NS_UINT16;
#endif

#ifndef NS_INT32
typedef long NS_INT32;
#endif

#ifndef NS_UINT32
typedef unsigned long NS_UINT32;
#endif

#ifndef NS_STR
typedef char * NS_STR;
#endif

#ifndef NS_CHAR
typedef char NS_CHAR;
#endif

#ifndef NS_BOOL
typedef unsigned char NS_BOOL;
#endif

#ifndef NS_TRUE
#define NS_TRUE (1)
#endif

#ifndef NS_FALSE
#define NS_FALSE (0)
#endif

#ifndef NS_VOID
typedef void NS_VOID;
#endif

#ifndef NS_NULL
#define NS_NULL (0)
#endif

#ifndef NS_ALGO_INDEX
typedef float NS_ALGO_INDEX;
#endif

#ifndef NS_INVALID_HANDLE
#define NS_INVALID_HANDLE       -1
#endif

#ifndef NS_PATH_DELIMITER
#define NS_PATH_DELIMITER       "\\"
#endif

typedef void*(*np_task_func)(void *arg);

typedef NS_INT32 np_task;
           
typedef NS_INT32 np_semaphore;
           
typedef NS_INT32 np_queue;
           
typedef NS_INT32 np_file;

#define _DEBUG
#ifdef _DEBUG
#include <Windows.h>
#define LOGI
#else
#define LOGI
#endif  /* _DEBUG */

#ifdef __cplusplus
}
#endif

#endif /* __PLATFORM_DEFINES_H */

  /************************ (C) COPYRIGHT NeuroSky Inc. *****END OF FILE****/
