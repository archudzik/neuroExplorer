var data = {
    "devices":
    {
        "eyetracker": {
            "id": "eyetracker",
            "status": "unknown",
            "selected": false,
            "title": {
                "selector": "Eye Tracker",
                "tab": "Eyes"
            },
            "controller": {
                "endpoint": "eyetracker",
                "template": "template-preview-eyetracker"
            }
        },
        "eeg": {
            "id": "eeg",
            "status": "unknown",
            "selected": false,
            "title": {
                "selector": "EEG",
                "tab": "EEG"
            },
            "controller": {
                "endpoint": "eeg",
                "template": "template-preview-eeg"
            }
        },
        "gsr": {
            "id": "gsr",
            "status": "unknown",
            "selected": false,
            "title": {
                "selector": "GSR",
                "tab": "GSR"
            },
            "controller": {
                "endpoint": "gsr",
                "template": "template-preview-gsr"
            }
        },
        "leap": {
            "id": "leap",
            "status": "unknown",
            "selected": false,
            "title": {
                "selector": "Leap Motion",
                "tab": "Hands"
            },
            "controller": {
                "endpoint": "leap",
                "template": "template-preview-leap"
            }
        },
        "mic": {
            "id": "mic",
            "status": "unknown",
            "selected": false,
            "title": {
                "selector": "Microphone",
                "tab": "Voice"
            },
            "controller": {
                "endpoint": "mic",
                "template": "template-preview-mic"
            }
        },
        "openface": {
            "id": "openface",
            "status": "unknown",
            "selected": false,
            "title": {
                "selector": "Face Analysis",
                "tab": "Face"
            },
            "controller": {
                "endpoint": "openface",
                "template": "template-preview-openface"
            }
        },
        "pulse": {
            "id": "pulse",
            "status": "unknown",
            "selected": false,
            "title": {
                "selector": "Pulse Oximetry",
                "tab": "Pulse"
            },
            "controller": {
                "endpoint": "pulse",
                "template": "template-preview-pulse"
            }
        },
        "dynamometer": {
            "id": "dynamometer",
            "status": "unknown",
            "selected": false,
            "title": {
                "selector": "Hand Dynamometer",
                "tab": "Dynamometer"
            },
            "controller": {
                "endpoint": "Dynamometer",
                "template": "template-preview-dynamometer"
            }
        },
    },
    "experiments":
    {
        "eyetracking": {
            "id": "eyetracking",
            "meta": {
                "h1": "Saccades",
                "h2": "Eyes research",
                "h3": "Saccades",
                "icon": "ti ti-eye"
            },
            "settings": {
                "stages": "auto"
            }
        },
        "emotions": {
            "id": "emotions",
            "meta": {
                "h1": "Emotions",
                "h2": "Emotions research",
                "h3": "Emotions",
                "icon": "ti ti-heart"
            },
            "settings": {
                "stages": "auto"
            }
        },
        "hands": {
            "id": "hands",
            "meta": {
                "h1": "Hands",
                "h2": "Movement research",
                "h3": "Hands",
                "icon": "ti ti-hand-open"
            },
            "settings": {
                "stages": "manual"
            }
        },
        "voice": {
            "id": "voice",
            "meta": {
                "h1": "Voice",
                "h2": "Voice recording",
                "h3": "Voice",
                "icon": "ti ti-microphone"
            },
            "settings": {
                "stages": "manual"
            }
        }
    },
    "patients":
    {
    }
};