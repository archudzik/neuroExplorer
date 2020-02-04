If this is your first time using the EEG Algorithm SDK for OS X, please start
by reading the "eeg_algorithm_sdk_for_windows_development_guide" PDF.  It will
tell you everything you need to know to get started.

If you need further help, please visit http://developer.neurosky.com for the latest
additional information.

To contact NeuroSky for support, please visit http://support.neurosky.com, or
send email to support@neurosky.com.

For developer community support, please visit our community forum on
http://www.linkedin.com/groups/NeuroSky-Brain-Computer-Interface-Technology-3572341

Happy coding!


SDK Contents
------------
- [README.txt]: This readme file
- [Algo SDK Sample]: Algo SDK sample project
- [Include]: Header files for of EEG Algo SDK
- [Libs]: EEG Algo SDK libraries
	- [AlgoSdkDll64.dll]: EEG Algo SDK DLL for x64 platform
	- [AlgoSdkDll.dll]: EEG Algo SDK DLL for x86 platform
- [eeg_algorithm_sdk_for_windows_development_guide.pdf]: EEG Algo SDK Windows development guide


Version History
---------------
2016-02-03: version 2.9.2
- supports EEG bandpower

2015-12-11: version 2.9.0
- special build that only supports Attention, Meditation and Eye Blink Detection


2015-12-09: version 2.2.2
- supports Eye Blink Detection

2015-10-22: version 2.2.1
- added “Suggested baseline data collection protocol”
- fixed issues: #1814, #1815, #1830

2015-10-08: version 2.2
- first release for Windows platform
- supports Attention, Meditation and Appreciation, Mental Effort and its secondary algorithm, Familiarity and its secondary algorithm


//////////////////////////////////////////////////////////////////////////////////////
// In the calculation of Appreciation algorithm, part of libSVM has been used.      //
// Below is the libSVM copyright terms and conditions                               //
//////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2000-2014 Chih-Chung Chang and Chih-Jen Lin                        //
// All rights reserved.                                                             //
//                                                                                  //
// Redistribution and use in source and binary forms, with or without               //
// modification, are permitted provided that the following conditions               //
// are met:                                                                         //
//                                                                                  //
// 1. Redistributions of source code must retain the above copyright                //
// notice, this list of conditions and the following disclaimer.                    //
//                                                                                  //
// 2. Redistributions in binary form must reproduce the above copyright             //
// notice, this list of conditions and the following disclaimer in the              //
// documentation and/or other materials provided with the distribution.             //
//                                                                                  //
// 3. Neither name of copyright holders nor the names of its contributors           //
// may be used to endorse or promote products derived from this software            //
// without specific prior written permission.                                       //
//                                                                                  //
//                                                                                  //
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS              //
// ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT              //
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR            //
// A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR           //
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,            //
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,              //
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR               //
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF           //
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING             //
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS               //
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.                     //
//////////////////////////////////////////////////////////////////////////////////////