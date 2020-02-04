"Algo SDK Sample" is a sample project to demonstrate how to manipulate data from MindWare Mobile Headset (realtime mode) or canned data (offline mode) and pass to EEG Algo SDK for EEG algorithm analysis. This document explains the use of "AlgoSdkDll.dll".

Running on Windows device (Realtime mode)
=========================================
1. Pairing NeuroSky MindWave Mobile Headset with Windows machine
2. Check the connected COM port number (e.g. COM10) and edit the define "MWM_COM" in "<SDK_PACKAGE>\Algo SDK Sample\Algo SDK Sample\Algo SDK Sample.cpp" to be the same as the connected port number (e.g. "COM10")
3. Double click the Algo SDK Sample Project Solution (Algo SDK Sample.sln) to launch the project with Visual Studio (e.g. Visual Studio Express 2015)
4. Select "Build" –> "Rebuild Project" to rebuild the Algo SDK Sample application
5. In the application,
	5.1. select algorithm(s) from top right corner and press "Init" button to initialise EEG Algo SDK (by invoking "NSK_ALGO_Init()" function)
	5.2. press "Start" button to start process any incoming headset data (by invoking "NSK_ALGO_Start()" function)
	5.3. press "Pause" button to pause EEG Algo SDK (by invoking "NSK_ALGO_Pause()" function)
	5.4. press "Stop" button to stop EEG Algo SDK (by invoking "NSK_ALGO_Stop()" function)

Running on Windows device (Offline mode)
========================================
1. Double click the Algo SDK Sample Project Solution (Algo SDK Sample.sln) to launch the project with Visual Studio (e.g. Visual Studio Express 2015)
2. Select "Build" –> "Rebuild Project" to rebuild the Algo SDK Sample application
3. In the application,
	3.1. select algorithm(s) from top right corner and press "Init" button to initialise EEG Algo SDK (by invoking "NSK_ALGO_Init()" function)
	3.2. press "Start" button to start process any incoming headset data (by invoking "NSK_ALGO_Start()" function)
	3.3. press "Data" button to start feeding offline data
	3.4. press "Pause" button to pause EEG Algo SDK (by invoking "NSK_ALGO_Pause()" function)
	3.5. press "Stop" button to stop EEG Algo SDK (by invoking "NSK_ALGO_Stop()" function)