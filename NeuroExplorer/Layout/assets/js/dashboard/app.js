class Application {

    constructor(pageName) {

        this.state = {
            patientId: undefined,
            examinationId: undefined,
            experimentId: undefined,
            experimentName: undefined,
            researcher: {
                firstname: undefined,
                lastname: undefined
            },
            location: {
                lat: undefined,
                lng: undefined,
                found: false
            },
            weather: {},
            selectedDevices: undefined,
            examinationStarted: undefined,
            pageName: undefined,
            charts: {},
            leap: {},
            logMessages: false,
            isRecording: false
        };

        if (pageName) {
            this.setPage(pageName);
        };

        this.jpegDecoder = new JpegDecoder();

        this.registerEvents();

        let self = this;
        this.websocket = new WebSocketController({
            "endpoint": "/cmd",
            "callbacks": {
                "onmessage": function (evt) {
                    let msg = JSON.parse(evt.data);
                    if (msg["to"] !== "dashboard" && msg["to"] !== "all") {
                        return;
                    }
                    if (self.state.logMessages) {
                        console.log("↓", msg);
                    }
                    self.getMessage(msg);
                },
                "onerror": function (evt) {
                    warn("Connection with server lost");
                },
                "onopen": function (evt) {
                    self.init();
                }
            }
        });
    }

    init() {
        getPatients();
        refreshDevices();
        refreshExperiments();
    }

    registerEvents() {
        let self = this;
        window.addEventListener('SET_STATE', function (e) {
            self.setState(e.detail);
        });
    }

    sendMessage(to, action, value) {
        let msg = {
            "to": to,
            "from": "dashboard",
            "action": action,
            "value": value
        };
        if (this.state.logMessages) {
            console.log("↑", msg);
        }
        this.websocket.send(msg);
    }

    getWeater() {
        let self = this;
        let weatherUrl = "https://api.openweathermap.org/data/2.5/weather"
            + "?lat=" + self.state.location.lat
            + "&lon=" + self.state.location.lng
            + "&units=metric"
            + "&appid=9291d5a98b80e4a2bc0dbdb3590aafdd";
        fetch(weatherUrl)
            .then((response, status) => {
                return response.json()
            })
            .then((weatherJson) => {
                self.setState({
                    "weather": {
                        "params": weatherJson.main,
                        "wind": weatherJson.wind,
                        "desc": weatherJson.weather
                    }
                });
                self.propagateMetadata();
            }, (error) => {
                console.log("weather error 1");
            })
            .catch(function () {
                console.log("weather error 2");
                self.propagateMetadata();
            });
    }

    getMessage(msg) {
        switch (msg["action"]) {
            case "stages":
                refreshStages(msg["value"]);
                break;

            case "progress":
                updateStage(msg["value"]);
                break;

            case "status":
                devicesStatusHandler(msg["value"]);
                break;

            case "recording":
                examinationRecordingHandler(msg["value"]);
                break;

            case "patients":
                getPatients(msg["value"]);
                break;

            case "location":
                if (!this.state.location.found) {
                    this.setState({ "location": { "lat": msg["value"].lat, "lng": msg["value"].lng, "found": true } });
                    this.getWeater();
                }
                break;
        }
    }

    setState(newState) {
        let self = this;
        if (typeof newState["experimentId"] !== "undefined") {
            if (newState["experimentId"] !== self.state.experimentId) {
                self.sendMessage("experiment", "set", newState.experimentId);
                if (newState["experimentId"] === "idle") {
                    self.state["experimentId"] = undefined;
                    self.state["experimentName"] = undefined;
                }
            }
        }
        self.state = _.merge(self.state, newState);
    }

    setStage(stageIndex) {
        this.sendMessage("experiment", "stage", stageIndex);
    }

    setPage(pageName) {
        this.state.pageName = pageName;
        let page = jQuery('.page-' + pageName)
        if (!page.length) {
            console.error(pageName, 'not found');
            return;
        }
        jQuery('.page-title-h1').text(page.data('title'));
        jQuery('.page').addClass('hidden');
        page.removeClass('hidden');
        if (page.data('hide-menu')) {
            $('.right-panel').addClass('right-panel-without-header');
        } else {
            $('.right-panel').removeClass('right-panel-without-header');
        }
        $("html, body").animate({ scrollTop: 0 }, "fast");
        $('#container-leap').addClass('hidden');
    }

    propagateMetadata() {
        let self = this;
        if (!self.state.examinationId) {
            return;
        }
        let now = moment().format("YYYY-MM-DD HH:mm:ss");
        let metadata = {
            date: now,
            patientId: self.state.patientId,
            examinationId: self.state.examinationId,
            experimentId: self.state.experimentId,
            experimentName: self.state.experimentName,
            researcher: {
                firstname: self.state.researcher.firstname,
                lastname: self.state.researcher.lastname
            },
            location: {
                lat: self.state.location.lat,
                lng: self.state.location.lng,
                found: self.state.location.found
            },
            weather: self.state.weather,
            notes: data.patients[self.state.patientId]['history'][self.state.examinationId]['notes']
        }
        self.sendMessage("controller", "metadata", metadata);
    }

}