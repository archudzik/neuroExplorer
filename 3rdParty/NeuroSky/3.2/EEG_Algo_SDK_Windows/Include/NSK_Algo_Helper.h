/**
 ******************************************************************************
 * @file    NSK_Algo_Helper.h
 * @author  Algo SDK Team
 * @version V0.1
 * @date    28-April-2015
 * @brief   Algo SDK Helper API declarations
 ******************************************************************************
 * @attention
 *
 * <h2><center>&copy; COPYRIGHT(c) NeuroSky Inc. All rights reserved.</center></h2>
 *
 *
 ******************************************************************************
 */

#ifndef __NSK_ALGO_HELPER_H_
#define __NSK_ALGO_HELPER_H_

#undef DECL_ENUM_ELEMENT
#undef BEGIN_ENUM
#undef END_ENUM

#ifndef GENERATE_ENUM_STRINGS
    #define DECL_ENUM_ELEMENT( element ) element
    #define BEGIN_ENUM( ENUM_NAME ) typedef enum ENUM_NAME
    #define END_ENUM( ENUM_NAME ) ENUM_NAME; \
                                char* GetString##ENUM_NAME(int index);
#else
    #define DECL_ENUM_ELEMENT( element ) #element
    #define BEGIN_ENUM( ENUM_NAME ) char* gs_##ENUM_NAME [] =
    #define END_ENUM( ENUM_NAME ) ; char* GetString##ENUM_NAME( \
                                int index){ return gs__##ENUM_NAME [index]; }
#endif

#endif  /* __NSK_ALGO_HELPER_H_ */
