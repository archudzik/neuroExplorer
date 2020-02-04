//
//  NSK_Algo_ReturnCodes.h
//  EEG Algo SDK
//
//  Created by Donald on 28/4/15.
//  Copyright (c) 2015 NeuroSky. All rights reserved.
//

#if ( !defined(__NSK_ALGO_RETURNCODES_H) || defined(GENERATE_ENUM_STRINGS) )

#if (!defined(GENERATE_ENUM_STRINGS))
#define __NSK_ALGO_RETURNCODES_H
#endif

#include "NSK_Algo_Helper.h"

BEGIN_ENUM(_eNSK_ALGO_RET) {
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_SUCCESS),                /* Operation performed successfully */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_FAIL),					/* Operation performed with error */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_ALREADY_INITED),         /* Already inited */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_ALREADY_STARTED),        /* SDK has already started data analysis or collectin baseline data */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_ALREADY_STOPPED),        /* SDK has already been stopped */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_NOT_INITED),             /* SDK hasn't been inited */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_NO_MEM),                 /* Not enough of memory */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_INVALID_PARAM),			/* Invalid input parameter */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_NOT_STARTED),            /* SDK hasn't been started yet */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_NOT_SUPPORTED),			/* Unsupported algorithm */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_NOT_SELECTED),           /* Unselected algorithm */
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_NO_CALLBACK),			/* No callback registered */
#ifdef EVALUATION_BUILD
    DECL_ENUM_ELEMENT(NSK_ALGO_RET_EXPIRED),                /* Evaluation build expired */
#endif
} END_ENUM(eNSK_ALGO_RET)

#endif  /* ( !defined(__NSK_ALGO_RETURNCODES_H) || defined(GENERATE_ENUM_STRINGS) ) */
