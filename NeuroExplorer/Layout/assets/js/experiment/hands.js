class HandsExperiment {

    constructor(callbacks) {
        let self = this;
        this.currentConfig = 0;
        this.stages = [
            { "name": "Left Hand Finger Tapping Test", "file": "assets/stimulus/hands/slide/Slide1.JPG" },
            { "name": "Right Hand Finger Tapping Test", "file": "assets/stimulus/hands/slide/Slide2.JPG" },
            { "name": "Left Hand Twisting", "file": "assets/stimulus/hands/slide/Slide3.JPG" },
            { "name": "Right Hand Twisting", "file": "assets/stimulus/hands/slide/Slide4.JPG" }
        ]
        this.callbacks = {
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

    stage(index, skipAnnotation) {
        this.currentConfig = index;
        $('#stimulus-hands').attr('src', this.stages[this.currentConfig].file);
        this.callbacks["progress"](this.currentConfig, 100);
        if (!skipAnnotation) {
            this.annotateStage();
        }
    }

    annotateStage() {
        if (typeof this.callbacks["annotate"] === "function") {
            this.callbacks["annotate"](this.currentConfig, { "caption": this.stages[this.currentConfig].name });
        }
    }

    allStages() {
        let result = [];
        let self = this;
        _.each(this.stages, function (stage, index) {
            result.push({ "id": index, "current": index === self.currentConfig, "clickable": true, "name": stage.name });
        });
        return result;
    }

    start() {
    }

    stop() {
    }

    reset() {
        this.stage(0);
    }

    masterIdle() {
        this.stage(0, true);
    }

}