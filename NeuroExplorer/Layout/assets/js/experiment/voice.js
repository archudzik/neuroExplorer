class VoiceExperiment {

    constructor(callbacks) {
        let self = this;
        this.currentConfig = 0;
        this.stages = [
            { "name": "Task01", "file": "assets/stimulus/voice/slide/Slide1.JPG" },
            { "name": "Task02", "file": "assets/stimulus/voice/slide/Slide2.JPG" },
            { "name": "Task03", "file": "assets/stimulus/voice/slide/Slide3.JPG" },
            { "name": "Task04", "file": "assets/stimulus/voice/slide/Slide4.JPG" },
            { "name": "Task05", "file": "assets/stimulus/voice/slide/Slide5.JPG" },
            { "name": "Task06", "file": "assets/stimulus/voice/slide/Slide6.JPG" },
            { "name": "Task07", "file": "assets/stimulus/voice/slide/Slide7.JPG" },
            { "name": "Task08", "file": "assets/stimulus/voice/slide/Slide8.JPG" },
            { "name": "Task09", "file": "assets/stimulus/voice/slide/Slide9.JPG" },
            { "name": "Task10", "file": "assets/stimulus/voice/slide/Slide10.JPG" },
            { "name": "Task11", "file": "assets/stimulus/voice/slide/Slide11.JPG" },
            { "name": "Task12", "file": "assets/stimulus/voice/slide/Slide12.JPG" },
            { "name": "Task13", "file": "assets/stimulus/voice/slide/Slide13.JPG" },
            { "name": "Task14", "file": "assets/stimulus/voice/slide/Slide14.JPG" },
            { "name": "Task15", "file": "assets/stimulus/voice/slide/Slide15.JPG" },
            { "name": "Task16", "file": "assets/stimulus/voice/slide/Slide16.JPG" },
            { "name": "Task17", "file": "assets/stimulus/voice/slide/Slide17.JPG" },
            { "name": "Task18", "file": "assets/stimulus/voice/slide/Slide18.JPG" },
            { "name": "Task19", "file": "assets/stimulus/voice/slide/Slide19.JPG" },
            { "name": "Task20", "file": "assets/stimulus/voice/slide/Slide20.JPG" },
            { "name": "Task21", "file": "assets/stimulus/voice/slide/Slide21.JPG" },
            { "name": "Task22", "file": "assets/stimulus/voice/slide/Slide22.JPG" },
            { "name": "Task23", "file": "assets/stimulus/voice/slide/Slide23.JPG" },
            { "name": "Task24", "file": "assets/stimulus/voice/slide/Slide24.JPG" },
            { "name": "Task25", "file": "assets/stimulus/voice/slide/Slide25.JPG" },
            { "name": "Task26", "file": "assets/stimulus/voice/slide/Slide26.JPG" },
            { "name": "Task27", "file": "assets/stimulus/voice/slide/Slide27.JPG" },
            { "name": "Task28", "file": "assets/stimulus/voice/slide/Slide28.JPG" },
            { "name": "Task29", "file": "assets/stimulus/voice/slide/Slide29.JPG" },
            { "name": "Task30", "file": "assets/stimulus/voice/slide/Slide30.JPG" },
            { "name": "Task31", "file": "assets/stimulus/voice/slide/Slide31.JPG" },
            { "name": "Task32", "file": "assets/stimulus/voice/slide/Slide32.JPG" },
            { "name": "Task33", "file": "assets/stimulus/voice/slide/Slide33.JPG" },
            { "name": "Task34", "file": "assets/stimulus/voice/slide/Slide34.JPG" },
            { "name": "Task35", "file": "assets/stimulus/voice/slide/Slide35.JPG" },
            { "name": "Task36", "file": "assets/stimulus/voice/slide/Slide36.JPG" },
            { "name": "Task37", "file": "assets/stimulus/voice/slide/Slide37.JPG" }
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
        $('#stimulus-voice').attr('src', this.stages[this.currentConfig].file);
        this.callbacks["progress"](this.currentConfig, 100);
        if (!skipAnnotation) {
            this.annotateStage();
        }
    }

    annotateStage() {
        if (typeof this.callbacks["annotate"] === "function") {
            if (!this.stages[this.currentConfig].notified) {
                this.stages[this.currentConfig].notified = true;
                this.callbacks["annotate"](this.currentConfig, { "caption": this.stages[this.currentConfig].name });
            }
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

    masterIdle(){
        this.stage(0, true);
    }

}