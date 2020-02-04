# Neuro Explorer 0.1.0 (beta)
## Universal system of data acquisition for digital biomarkers

Neuro Explorer is a system, which goal is the data acquisition for digital biomarkers of neurodegenerative diseases that would aggregate data from various biosensors.

![Neuro Explorer logo](https://neuroexplorer.online/static/github/logo.png)

Neuro Explorer strongly relies on one of the best research open-source projects, the OpenFace software (https://github.com/TadasBaltrusaitis/OpenFace). OpenFace is the ﬁrst toolkit capable of facial landmark detection, head pose estimation, facial action unit recognition, and eye-gaze estimation with available source code for both running and training the models. Our goal is to extend the capabilities of that project with additional devices.

# Functionality

The researcher can conduct every experiment with a chosen number of devices. The current list supports eight sensors: 
* Eye Tracker (TheEyeTribe, owned by Facebook)
* EEG (NeuroSky), GSR (ThoughtStream)
* Leap Motion
* Microphone (every audio interface)
* Face Analysis (based on the video camera)
* Pulse Oximetry (Contec)
* Hand Dynamometer (Vernier). 

Furthermore the system, thanks to OpenFace, is capable of performing a number of facial analysis tasks:

* Facial Landmark Detection
* Facial Landmark and head pose tracking (links to YouTube videos)
* Facial Action Unit Recognition
* Gaze tracking (image of it in action)
* Facial Feature Extraction (aligned faces and HOG features)

A significant factor of that software is its elasticity. For example, if given Research Organisation does not have an eye-tracker, it still can use this software without any modifications. The researcher can experiment with classical webcam and even register eye movements. For Facial Expression Recognition, just as previously, even camera built-in laptop will produce some results. For Voice Analysis, most of the standard microphones are supported. Neuro Explorer is automatically detecting supported hardware, and if it is not available, it will not be used. Although more powerful hardware, more accurate results.

# Experiments

The current version allows conducting four different experiments. 
* "Saccades" allows measuring saccades and antisaccades based on well-tested protocol. 
* "Emotions" displays the stimulus in the form of the video presenting various, spontaneous emotions.
* "Hands" consists of the tasks related to hand movements and grip strength. 
* "Voice" displays slides (currently: in the Polish language) that lead the patient through vocabulary examinations. 

# Compilation

Startup project is OpenFace/NeuroExplorer_OpenFace.sln Solution File.
We recommend using Microsoft Visual Studio 2017.

# Copyright
Academic or non-profit organization non-commercial research use only.

You have to respect OpenFace (OpenFace/Copyright.txt), dlib, OpenBLAS, and OpenCV licenses.

Furthermore you have to respect the licenses of the datasets used for model training - https://github.com/TadasBaltrusaitis/OpenFace/wiki/Datasets
