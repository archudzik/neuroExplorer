class EyeTrackingExperiment {

    constructor(callbacks) {
        let self = this;
        this.gazePoint = new GazePoint();
        this.currentConfig = 0;
        this.callbacks = callbacks;
        this.stages =
            [
                {
                    name: "0.125Hz",
                    notified: false,
                    amplitude: 600,
                    period: 8000,
                    times: 10,
                    color: "#00ff00",
                    movement: "pursuit",
                    shape: "rect",
                    steps: [],
                    callbacks: {
                        "progress": function (stageId, data) {
                            callbacks["progress"](stageId, data);
                        },
                        "done": function (stageId, data) {
                            callbacks["done"](stageId, data);
                            self.stage(1);
                            self.start();
                        },
                        "annotate": function (stageId, data) {
                            callbacks["annotate"](stageId, data);
                        }
                    }
                },
                {
                    name: "0.25Hz",
                    notified: false,
                    amplitude: 600,
                    period: 4000,
                    times: 10,
                    color: "#00ff00",
                    movement: "pursuit",
                    shape: "rect",
                    steps: [],
                    callbacks: {
                        "progress": function (stageId, data) {
                            callbacks["progress"](stageId, data);
                        },
                        "done": function (stageId, data) {
                            callbacks["done"](stageId, data);
                            self.stage(2);
                            self.start();
                        },
                        "annotate": function (stageId, data) {
                            callbacks["annotate"](stageId, data);
                        }
                    }
                },
                {
                    name: "0.50Hz",
                    notified: false,
                    amplitude: 600,
                    period: 2000,
                    times: 10,
                    color: "#00ff00",
                    movement: "pursuit",
                    shape: "rect",
                    steps: [],
                    callbacks: {
                        "progress": function (stageId, data) {
                            callbacks["progress"](stageId, data);
                        },
                        "done": function (stageId, data) {
                            callbacks["done"](stageId, data);
                            self.stage(3);
                            self.start();
                        },
                        "annotate": function (stageId, data) {
                            callbacks["annotate"](stageId, data);
                        }
                    }
                },
                {
                    name: "Saccades",
                    notified: false,
                    amplitude: 600,
                    period: 2000,
                    times: 8,
                    color: "#00ff00",
                    movement: "saccade",
                    shape: "rect",
                    fixations: [0, 1, 0, 1, 0, 1, 0, 1, 0, -1, 0, 1, 0, 1, 0, -1, 0, -1, 0, -1, 0],
                    callbacks: {
                        "progress": function (stageId, data) {
                            callbacks["progress"](stageId, data);
                        },
                        "done": function (stageId, data) {
                            callbacks["done"](stageId, data);
                            self.stage(4);
                            self.start();
                        },
                        "annotate": function (stageId, data) {
                            callbacks["annotate"](stageId, data);
                        }
                    }
                },
                {
                    name: "Anti-saccades",
                    notified: false,
                    amplitude: 600,
                    period: 2000,
                    times: 8,
                    color: "#ff0000",
                    movement: "saccade",
                    shape: "rect",
                    fixations: [0, 1, 0, 1, 0, 1, 0, 1, 0, -1, 0, 1, 0, 1, 0, -1, 0, 1, 0, -1, 0],
                    callbacks: {
                        "progress": function (stageId, data) {
                            callbacks["progress"](stageId, data);
                        },
                        "done": function (stageId, data) {
                            callbacks["done"](stageId, data);
                        },
                        "annotate": function (stageId, data) {
                            callbacks["annotate"](stageId, data);
                        }
                    }
                }
            ];
    }

    stage(index, skipAnnotation) {
        this.currentConfig = index;
        this.gazePoint.init(this.currentConfig, this.stages[this.currentConfig]);
        if(!skipAnnotation){
            this.annotateStage();
        }
    }

    annotateStage() {
        if (typeof this.callbacks["annotate"] === "function") {
            if (!this.stages[this.currentConfig].notified){
                this.stages[this.currentConfig].notified = true;
                this.callbacks["annotate"](this.currentConfig, { "caption": this.stages[this.currentConfig].name });
            }
        }
    }

    allStages() {
        let result = [];
        let self = this;
        _.each(this.stages, function (stage, index) {
            result.push({ "id": index, "current": index === self.currentConfig, "name": stage.name });
        });
        return result;
    }

    start() {
        this.gazePoint.start();
    }

    stop() {
        this.gazePoint.stop();
    }

    reset() {
        this.stop();
        _.each(this.stages, function(stage, index){
            stage.notified = false;
        });
        this.stage(0);
    }

    masterIdle(){
        this.stop();
        _.each(this.stages, function (stage, index) {
            stage.notified = false;
        });
        this.stage(0, true);
    }

}
