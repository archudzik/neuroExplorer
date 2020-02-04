/**
  ******************************************************************************
  * @file    NSK_Algo.h 
  * @author  Algo SDK Team
  * @version V0.1
  * @date    15-April-2015
  * @brief   Algo SDK API declarations
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; COPYRIGHT(c) NeuroSky Inc. All rights reserved.</center></h2>
  *
  *
  ******************************************************************************
  */

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __NSK_ALGO_H_
#define __NSK_ALGO_H_

#ifdef __cplusplus
 extern "C" {
#endif
   
/* Includes ------------------------------------------------------------------*/
#include "NSK_Algo_Defines.h"

/* Exported types ------------------------------------------------------------*/
/**
  * @brief  Type definition of SDK callback.
  *			Note: Callback function implementation should return immediately 
  * @param  param	: SDK reported parameter.
  * @retval Nil.
  */
typedef NS_VOID(*NskAlgo_Callback)(sNSK_ALGO_CB_PARAM param);

/* Exported variables --------------------------------------------------------*/

/* Exported constants --------------------------------------------------------*/

/* Exported macros -----------------------------------------------------------*/

/* Exported functions ------------------------------------------------------- */
/**
  * @brief	Required: Initialize the Algo SDK with supported algorithm types.
  * @param	type        : Bit-wise OR the supported algorithm types
  * @param  dataPath    : An user data path to store user data, e.g. baseline data, user profiles etc.
  * @retval	NSK_ALGO_RET_SUCCESS on operation success or else fail
  */
NSK_ALGO_DECL(eNSK_ALGO_RET, NSK_ALGO_Init, (eNSK_ALGO_TYPE type, const NS_STR dataPath));

/**
  * @brief	Required: Uninitialize the Algo SDK
  *				Note: if SDK state is NSK_ALGO_STATE_RUNNING or NSK_ALGO_STATE_COLLECTING_BASELINE_DATA, then SDK state will change to NSK_ALGO_STATE_STOP with reason NSK_ALGO_REASON_BY_USER before SDK is uninitialized
  * @retval	NSK_ALGO_RET_SUCCESS on operation success or else fail
  */
NSK_ALGO_DECL(eNSK_ALGO_RET, NSK_ALGO_Uninit, (NS_VOID));

/**
  * @brief	Required: Register the callback for the host application to get notification on algorithm result, SDK state change and data signal quality
  *				Note: if SDK state is NSK_ALGO_STATE_RUNNING or NSK_ALGO_STATE_COLLECTING_BASELINE_DATA, then SDK state will be changed to NSK_ALGO_STATE_STOP with reason NSK_ALGO_REASON_CB_CHANGED before updating exiting callback (new callback will be invoked instead)
  *             Note: This function can be invoked before NSK_ALGO_Init() in order to collect the NSK_ALGO_STATE_INITED SDK state changed event.
  * @param	cbFunc      : Function pointer of the callback
  * @param  userData    : User data to be pass in the callback parameter
  * @retval	NSK_ALGO_RET_SUCCESS on operation success or else fail
  */
NSK_ALGO_DECL(eNSK_ALGO_RET, NSK_ALGO_RegisterCallback, (NskAlgo_Callback cbFunc, NS_VOID *userData));

/**
  * @brief	Optional: Get the Algo SDK version
  *				Format: M.m.p, where M is major version, m is minor version and p is patch version
  * @retval	Null terminated string
  */
NSK_ALGO_DECL(NS_STR, NSK_ALGO_SdkVersion, (NS_VOID));

/**
  * @brief	Optional: Get the Algo SDK version
  *				Format: M.m.p, where M is major version, m is minor version and p is patch version
  * @param	type	: Specify the supported Algo type version to be queried
  * @retval	Null terminated string
  */
NSK_ALGO_DECL(NS_STR, NSK_ALGO_AlgoVersion, (eNSK_ALGO_TYPE type));

/**
  * @brief	Required: Start processing data from NSK_ALGO_DataStream() call
  * @param	bBaseline	: [RESERVED] Must be always NS_FALSE
  * @retval	NSK_ALGO_RET_SUCCESS on operation success or else fail
  */
NSK_ALGO_DECL(eNSK_ALGO_RET, NSK_ALGO_Start, (NS_BOOL bBaseline));

/**
  * @brief	Required: Pause processing/collecting data
  *				- SDK state will changed to NSK_ALGO_STATE_PAUSE with reason NSK_ALGO_REASON_BY_USER
  * @retval	NSK_ALGO_RET_SUCCESS on operation success or else fail
  */
NSK_ALGO_DECL(eNSK_ALGO_RET, NSK_ALGO_Pause, (NS_VOID));

/**
  * @brief	Required: Stop processing/collecting data.
  *				- SDK state will changed to NSK_ALGO_STATE_STOP with reason NSK_ALGO_REASON_BY_USER
  * @retval	NSK_ALGO_RET_SUCCESS on operation success or else fail
  */
NSK_ALGO_DECL(eNSK_ALGO_RET, NSK_ALGO_Stop, (NS_VOID));

/**
  * @brief	Required: EEG data stream input
  *             When type = NSK_ALGO_DATA_TYPE_PQ, dataLength = 1
  *             When type = NSK_ALGO_DATA_TYPE_EEG, dataLength = 512 (i.e. 1 second EEG raw data)
  *             When type = NSK_ALGO_DATA_TYPE_ATT, dataLength = 1
  *             When type = NSK_ALGO_DATA_TYPE_MED, dataLength = 1
  *             When type = NSK_ALGO_DATA_TYPE_BULK_EEG, dataLength = N*512 (i.e. N continous seconds of EEG raw data)
  *             Note 1: In case of type = NSK_ALGO_DATA_TYPE_BULK_EEG, caller should NOT release the data buffer until SDK state changes back to NSK_ALGO_STATE_STOP
  *             Note 2: In case of type = NSK_ALGO_DATA_TYPE_BULK_EEG, the first 5 seconds of data will be used as baseline data
  * @param	type		: Type of the given data stream
  * @param	data		: Data stream
  * @param	dataLenght	: Size of the data stream
  * @retval	Nil
  */
NSK_ALGO_DECL(eNSK_ALGO_RET, NSK_ALGO_DataStream, (eNSK_ALGO_DATA_TYPE type, NS_INT16 *data, NS_INT dataLenght));

#ifdef __cplusplus
}
#endif

#endif /* __NSK_ALGO_H_ */

/************************ (C) COPYRIGHT NeuroSky Inc. *****END OF FILE****/
