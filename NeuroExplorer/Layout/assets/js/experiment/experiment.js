class Experiment {

    constructor() {
        let self = this;

        this.callbacks = {
            "progress": function (stageId, data) {
                self.websocket.send({ "to": "all", "action": "progress", "value": { "id": stageId, "data": data } });
            },
            "done": function (stageId, data) {
                self.websocket.send({ "to": "all", "action": "done", "value": { "id": stageId, "data": data } });
            },
            "annotate": function (stageId, data) {
                self.websocket.send({ "to": "all", "action": "annotate", "value": { "id": stageId, "data": data } });
            }
        }

        this.controller = undefined;
        this.controllers = {
            eyetracking: undefined,
            emotions: undefined,
            voice: undefined,
            hands: undefined
        }
        this.controllers.eyetracking = new EyeTrackingExperiment(this.callbacks);
        this.controllers.emotions = new EmotionsExperiment(this.callbacks);
        this.controllers.voice = new VoiceExperiment(this.callbacks);
        this.controllers.hands = new HandsExperiment(this.callbacks);

        this.websocket = new WebSocketController({
            "endpoint": "/cmd",
            "callbacks": {
                "onmessage": function (evt) {
                    let msg = JSON.parse(evt.data);
                    if (msg["to"] !== "experiment" && msg["to"] !== "all") {
                        return;
                    }
                    switch (msg["action"]) {
                        case "idle": self.idle(); break;
                        case "set": self.set(msg["value"]); break;
                        case "stage": self.stage(parseInt(msg["value"], 10)); break;
                        case "stages": self.stages(); break;
                        case "start": self.start(); break;
                        case "stop": self.stop(); break;
                        case "reset": self.reset(); break;
                    }
                }
            }
        });

    }

    ensure(experiment) {
        let self = this;
        if (typeof self.controllers[experiment] === "undefined") {
            console.error("controller is not defined");
            return false;
        };
        return true;
    }

    idle() {
        _.each(this.controllers, function (controller) {
            controller.masterIdle();
        });
        $('.fullscreen').addClass('hidden');
        $('.screen-idle').removeClass('hidden');
    }

    set(experiment) {
        console.log('set', experiment);
        if(experiment === "idle"){
            this.idle();
            return;
        }
        if (!this.ensure(experiment)) {
            return;
        }
        this.controller = experiment;
        this.idle();
        $('.fullscreen').addClass('hidden');
        $('.screen-' + experiment).removeClass('hidden');
        this.stage(0);
        this.stages();
    }

    stage(index) {
        if (!this.ensure(this.controller)) {
            return;
        }
        this.controllers[this.controller].stage(index);
    }

    stages() {
        if (!this.ensure(this.controller)) {
            return;
        }
        let stages = this.controllers[this.controller].allStages();
        this.websocket.send({ "to": "dashboard", "from" : "experiment", "action": "stages", "value": stages });
    }

    start() {
        if (!this.ensure(this.controller)) {
            return;
        }
        this.controllers[this.controller].start();
    }

    stop() {
        if (!this.ensure(this.controller)) {
            return;
        }
        this.controllers[this.controller].stop();
    }

    reset() {
        if (!this.ensure(this.controller)) {
            return;
        }
        this.controllers[this.controller].reset();
    }

}