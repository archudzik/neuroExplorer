/**
  ******************************************************************************
  * @file    NSK_Algo_DataType.h 
  * @author  Algo SDK Team
  * @version V0.1
  * @date    14-April-2015
  * @brief   Algo SDK data type declarations
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; COPYRIGHT(c) NeuroSky Inc. All rights reserved.</center></h2>
  *
  *
  ******************************************************************************
  */

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __NSK_ALGO_DEFINES_H_
#define __NSK_ALGO_DEFINES_H_

#ifdef __cplusplus
 extern "C" {
#endif
   
/* Includes ------------------------------------------------------------------*/
#include "Platform_Defines.h"
#include "NSK_Algo_ReturnCodes.h"

#ifndef __PLATFORM_DEFINES_H
	#error Missing heading: Platform data type header file must be included Platform_DataType.h \
			NSK_ALGO_DECL(ret, func, args)		ret func args \
				e.g. in Windows DLL, NSK_ALGO_DECL may need to declare as: \
					#define NSK_ALGO_DECL(ret, func, args)		__declspec(dllexport) ret __cdecl func args \
			NS_INT -> int \
			NS_UINT -> unsigned int \
			NS_INT8 -> char \
			NS_UINT8 -> unsigned char \
			NS_INT16 -> short \
			NS_UINT16 -> unsigned short \
			NS_INT32 -> long \
			NS_UINT32 -> unsigned long \
			NS_STR -> char * \
			NS_BOOL -> bool \
				NS_TRUE -> true \
				NS_FALSE -> false \
			NS_VOID -> void \
			NS_NULL -> NULL \
			NS_ALGO_INDEX -> float
#endif

/* Exported types ------------------------------------------------------------*/
typedef enum _eNSK_ALGO_DATA_TYPE {
	NSK_ALGO_DATA_TYPE_EEG		= 0x01,		/* Raw EEG data */
    NSK_ALGO_DATA_TYPE_ATT,                 /* Attention data */
    NSK_ALGO_DATA_TYPE_MED,                 /* Meditation data */
	NSK_ALGO_DATA_TYPE_PQ,					/* Poor signal quality data */
#ifndef TRIAL_BUILD
    NSK_ALGO_DATA_TYPE_BULK_EEG,            /* Bulk of EEG data */
#endif	/* !TRIAL_BUILD */
	NSK_ALGO_DATA_TYPE_MAX
} eNSK_ALGO_DATA_TYPE;	/* This will only be used by COMM SDK */

typedef enum _eNSK_ALGO_PROFILE_ACTION {
	NSK_ALGO_PROFILE_ACTION_ADD = 0,		/* Add/Edit (if exists) user profile */
	NSK_ALGO_PROFILE_ACTION_REMOVE,			/* Remove existing user profile */
	NSK_ALGO_PROFILE_ACTION_GET,			/* Get registered user profiles */
	NSK_ALGO_PROFILE_ACTION_SET_ACTIVE		/* Set user profile to active profile */
} eNSK_ALGO_PROFILE_ACTION;

typedef enum _eNSK_ALGO_TYPE {
    NSK_ALGO_TYPE_ATT       = 0x00000100,   /* Attention */
    NSK_ALGO_TYPE_MED       = 0x00000200,   /* Meditation */
    NSK_ALGO_TYPE_BLINK     = 0x00000400,   /* Eye blink detection */
    NSK_ALGO_TYPE_BP        = 0x00004000    /* EEG Bandpower */
} eNSK_ALGO_TYPE;

typedef enum _eNSK_ALGO_CB_TYPE {
	NSK_ALGO_CB_TYPE_STATE			= 0x01000000,	/* Register callback for Algo SDK state change */
	NSK_ALGO_CB_TYPE_SIGNAL_LEVEL	= 0x02000000,	/* Register callback for Algo signal quality level */
	NSK_ALGO_CB_TYPE_ALGO			= 0x04000000	/* Register callback for Algo analysis result. This flag must be bit-wise OR with eNSK_ALGO_TYPE */
} eNSK_ALGO_CB_TYPE;

typedef enum _eNSK_ALGO_STATE {
	/* SDK state */
	NSK_ALGO_STATE_INITED						= 0x0100,	/* Algo SDK is initialized (Reason code is omitted), host application should never receive this state */
	NSK_ALGO_STATE_RUNNING						= 0x0200,	/* Algo SDK is performing analysis.
															 */
	NSK_ALGO_STATE_COLLECTING_BASELINE_DATA		= 0x0300,	/* Algo SDK is collecting baseline data (Reason code is omitted).
															 *
															 * When baseline data collection is done, SDK state should change 
															 * to NSK_ALGO_STATE_RUNNING and start data analysis
															 */
	NSK_ALGO_STATE_STOP							= 0x0400,	/* Algo SDK stops data analysis/baseline collection.
															 * State will only change to stop if previous state is NSK_ALGO_STATE_RUNNING or
															 * NSK_ALGO_STATE_COLLECTING_BASELINE_DATA
															 */
    NSK_ALGO_STATE_PAUSE                        = 0x0500,   /* Algo SDK pauses data analysis due to poor signal quality or paused by user.
                                                             * State will only change to pause if previous state is NSK_ALGO_STATE_RUNNING
                                                             */
	NSK_ALGO_STATE_UNINTIED						= 0x0600,	/* Algo SDK is uninitialized (Reason code is omitted) */
    NSK_ALGO_STATE_ANALYSING_BULK_DATA          = 0x0800,   /* Algo SDK is analysing provided bulk data (i.e. NSK_ALGO_DataStream() is invoked with NSK_ALGO_DATA_TYPE_BULK_EEG.
                                                             * Note: SDK state will change to NSK_ALGO_STATE_STOP after analysing data
                                                             */
    NSK_ALGO_STATE_RUNNING_DEMO                 = 0x1000,   /* Algo SDK is now in demo mode. (Only occurs in Trial Build)
                                                             * SDK will return non-accurate algorithm index randomly.
                                                             */
	
    NSK_ALGO_STATE_MASK                         = 0xFF00,
    
	/* Reason for state change */
	NSK_ALGO_REASON_CONFIG_CHANGED				= 0x0001,	/* RESERVED: SDK configuration changed (i.e. NSK_ALGO_Config() is invoked) */
	NSK_ALGO_REASON_USER_PROFILE_CHANGED		= 0x0002,	/* RESERVED: Active user profile has been changed (i.e. NSK_ALGO_Profile() is invoked and active user profile is affected) */
	NSK_ALGO_REASON_CB_CHANGED					= 0x0003,	/* RESERVED: Callback registration has been changed (i.e. NSK_ALGO_RegisterCallback() is invoked) */
	NSK_ALGO_REASON_BY_USER                     = 0x0004,	/* Stopped/Paused by user (i.e. NSK_ALGO_Stop()/NSK_ALGO_Pause() is invoked) */
	NSK_ALGO_REASON_BASELINE_EXPIRED			= 0x0005,	/* RESERVED: Active user baseline data expired. */
	NSK_ALGO_REASON_NO_BASELINE					= 0x0006,	/* RESERVED: There is no baseline data for current active user. */
	NSK_ALGO_REASON_SIGNAL_QUALITY				= 0x0007,	/* SDK state changes due to signal quality changes.
															 *
															 * e.g. NSK_ALGO_STATE_PAUSE + NSK_ALGO_REASON_SIGNAL_QUALITY means SDK pauses data analysis due to poor signal quality
															 * e.g. NSK_ALGO_STATE_RUNNING + NSK_ALGO_REASON_SIGNAL_QUALITY means SDK resumes data analysis due to signal resuming
															 * from poor signal quality
															 */
    NSK_ALGO_REASON_MASK                        = 0x00FF
} eNSK_ALGO_STATE;

typedef enum _eNSK_ALGO_SIGNAL_QUALITY {
	NSK_ALGO_SQ_GOOD = 0,		/* Good signal quality */
	NSK_ALGO_SQ_MEDIUM,			/* Medium signal quality */
	NSK_ALGO_SQ_POOR,			/* Poor signal quality */
	NSK_ALGO_SQ_NOT_DETECTED	/* No signal detected. It probably is caused by bad sensor contact */
} eNSK_ALGO_SIGNAL_QUALITY;

/**************************
 Algorithm return indexes definitions
 *************************/

/* RESERVED: Familiarity */
typedef struct _NS_ALGO_F_INDEX {
    NS_ALGO_INDEX abs_f;
    NS_ALGO_INDEX diff_f;
    NS_ALGO_INDEX max_f;
    NS_ALGO_INDEX min_f;
} NS_ALGO_F_INDEX;

/* RESERVED */
typedef enum _eNSK_ALGO_F2_PROGRESS_LEVEL {
    NSK_ALGO_F2_PROGRESS_LEVEL_VERY_POOR = 1,
    NSK_ALGO_F2_PROGRESS_LEVEL_POOR,
    NSK_ALGO_F2_PROGRESS_LEVEL_FLAT,
    NSK_ALGO_F2_PROGRESS_LEVEL_GOOD,
    NSK_ALGO_F2_PROGRESS_LEVEL_GREAT
} eNSK_ALGO_F2_PROGRESS_LEVEL;

/* RESERVED */
typedef struct _NS_ALGO_F2_INDEX {
    eNSK_ALGO_F2_PROGRESS_LEVEL progress_level;
    NS_ALGO_INDEX f_degree;
} NS_ALGO_F2_INDEX;
     
/* RESERVED: Mental Effort */
typedef struct _NS_ALGO_ME_INDEX {
    NS_ALGO_INDEX abs_me;
    NS_ALGO_INDEX diff_me;
    NS_ALGO_INDEX max_me;
    NS_ALGO_INDEX min_me;
} NS_ALGO_ME_INDEX;

/* RESERVED */
typedef struct _NS_ALGO_ME2_INDEX {
    NS_ALGO_INDEX total_me;
    NS_ALGO_INDEX me_rate;
    NS_ALGO_INDEX changing_rate;
} NS_ALGO_ME2_INDEX;

/* RESERVED */
typedef enum _NS_ALGO_BCQ_VALID_TYPE {
    NS_ALGO_BCQ_TYPE_VALUE = 1,
    NS_ALGO_BCQ_TYPE_VALID = 2,
    NS_ALGO_BCQ_TYPE_BOTH = 3
} NS_ALGO_BCQ_VALID_TYPE;

/* RESERVED */
/* Creativity */
typedef struct _NS_ALGO_CR_INDEX {
    NS_ALGO_BCQ_VALID_TYPE cr_index_type;
    NS_ALGO_INDEX cr_value;
    NS_INT8 BCQ_valid;
} NS_ALGO_CR_INDEX;

/* RESERVED */
/* Alertness */
typedef struct _NS_ALGO_AL_INDEX {
    NS_ALGO_BCQ_VALID_TYPE al_index_type;
    NS_ALGO_INDEX al_value;
    NS_INT8 BCQ_valid;
} NS_ALGO_AL_INDEX;

/* RESERVED */
/* Cognitive Preparedness */
typedef struct _NS_ALGO_CP_INDEX {
    NS_ALGO_BCQ_VALID_TYPE cp_index_type;
    NS_ALGO_INDEX cp_value;
    NS_INT8 BCQ_valid;
} NS_ALGO_CP_INDEX;

/* EEG Bandpower */
typedef struct _NS_ALGO_BP_INDEX {
    NS_ALGO_INDEX delta_power;
    NS_ALGO_INDEX theta_power;
    NS_ALGO_INDEX alpha_power;
    NS_ALGO_INDEX beta_power;
    NS_ALGO_INDEX gamma_power;
} NS_ALGO_BP_INDEX;

/* RESERVED */
/* Appreciation */
typedef NS_ALGO_INDEX NS_ALGO_AP_INDEX;
     
/* Attention */
typedef NS_ALGO_INDEX NS_ALGO_ATT_INDEX;
     
/* Meditation */
typedef NS_ALGO_INDEX NS_ALGO_MED_INDEX;
     
/* Eye blink detection */
typedef NS_ALGO_INDEX NS_ALGO_EYE_BLINK_STRENGTH;

typedef struct _sNSK_ALGO_INDEX_GROUP {
    union {
		NS_ALGO_AP_INDEX ap_index;		/* RESERVED */
        NS_ALGO_ME_INDEX me_index;		/* RESERVED */
        NS_ALGO_ME2_INDEX me2_index;	/* RESERVED */
        NS_ALGO_F_INDEX f_index;		/* RESERVED */
        NS_ALGO_F2_INDEX f2_index;		/* RESERVED */
        NS_ALGO_ATT_INDEX att_index;
        NS_ALGO_MED_INDEX med_index;
        NS_ALGO_EYE_BLINK_STRENGTH eye_blink_strength;
        NS_ALGO_CR_INDEX cr_index;		/* RESERVED */
        NS_ALGO_AL_INDEX al_index;		/* RESERVED */
        NS_ALGO_CP_INDEX cp_index;		/* RESERVED */
        NS_ALGO_BP_INDEX bp_index;
    } group;
} sNSK_ALGO_INDEX_GROUP;

typedef struct _sNSK_ALGO_INDEX {
	eNSK_ALGO_TYPE type;            /* Reported Algo type */
	sNSK_ALGO_INDEX_GROUP value;	/* Reported Algo index, depending on the eNSK_ALGO_TYPE */
	NS_INT32 timeSpent;
} sNSK_ALGO_INDEX;

typedef struct _sNSK_ALGO_CB_PARAM {
	/**
	  * Different cbType should map to corresponding param
	  * NSK_ALGO_CB_TYPE_STATE -> state
	  * eNSK_ALGO_SIGNAL_QUALITY -> sq
	  * NSK_ALGO_CB_TYPE_ALGO -> index
	  */
	eNSK_ALGO_CB_TYPE cbType;
	union {
		eNSK_ALGO_STATE state;			/* SDK reported state (MSB - State, LSB - Reason) */
		eNSK_ALGO_SIGNAL_QUALITY sq;	/* SDK reported signal quality level */
		sNSK_ALGO_INDEX index;			/* SDK reported algorithm index */
	} param;
    NS_VOID *userData;
} sNSK_ALGO_CB_PARAM;

/* Exported variables --------------------------------------------------------*/

/* Exported constants --------------------------------------------------------*/

/* Exported macros -----------------------------------------------------------*/

/* Exported functions ------------------------------------------------------- */ 

#ifdef __cplusplus
}
#endif

#endif /* __NSK_ALGO_DEFINES_H_ */

/************************ (C) COPYRIGHT NeuroSky Inc. *****END OF FILE****/
