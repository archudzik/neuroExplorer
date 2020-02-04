class GazePoint {

    constructor() {
        let self = this;
        self.initialized = false;
        self.animate();
        window.onresize = function (event) {
            self.setCanvasSize();
        };
    }

    init(stageId, config) {
        let self = this;
        this.stageId = stageId;
        this.initialized = true;
        this.movement = config.movement;
        this.callbacks = config.callbacks;
        this.canvas = document.getElementById("stimulus-gaze");
        this.context = this.canvas.getContext("2d");
        this.amplitude = config.amplitude;
        this.period = config.period;
        this.fixations = config.fixations;
        this.shape = config.shape;
        this.fixation = 0;
        this.circleSize = 30;
        this.sendReport = false;
        this.times = config.times + 1;
        this.counterZero = 0;
        this.counterMinus = 0;
        this.counterPlus = 0;
        this.step = 1000 / 60;
        this.tick = -this.step;
        this.visible = true;
        this.minimumVal = 1;
        this.running = false;
        this.lastSaccadeTime = performance.now();
        this.lastSaccadeValue = 0;
        this.stimFigure =
            {
                x: self.canvas.width / 2 - self.circleSize / 2,
                y: self.canvas.height / 2 - self.circleSize / 2,
                size: self.circleSize,
                color: config.color
            };
        self.setCanvasSize();
    }

    start() {
        this.running = true;
        this.sendReport = true;
    }

    stop() {
        this.running = false;
        this.sendReport = false;
    }

    reset() {
        this.stop();
        this.center();
        this.init();
    }

    report() {
        if (this.sendReport) {
            let self = this;
            let data = {
                meta: {
                    window: {
                        width: window.innerWidth,
                        height: window.innerHeight
                    }
                },
                data: self.stimFigure
            }
            if (typeof this.callbacks["annotate"] === "function") {
                this.callbacks["annotate"](this.stageId, data);
            }
        }
    }

    progress() {
        let self = this;
        let percent = 0;
        switch (self.movement) {
            case "pursuit":
                percent = Math.round(this.counterZero * 100 / this.times);
                break;

            case "saccade":
                percent = Math.round(this.fixation * 100 / this.fixations.length);
                break;
        }
        if (typeof this.callbacks["progress"] === "function") {
            this.callbacks["progress"](this.stageId, percent);
        }
        if (percent === 100) {
            this.stop();
            this.clear();
            if (typeof this.callbacks["done"] === "function") {
                this.callbacks["done"](this.stageId, percent);
            }
        }
    }

    center() {
        if (!this.initialized) {
            return;
        }
        this.stimFigure.x = this.canvas.width / 2 - this.circleSize / 2;
        this.stimFigure.y = this.canvas.height / 2 - this.circleSize / 2;
    }

    clear() {
        this.context.clearRect(0, 0, this.canvas.width, this.canvas.height);
    }

    draw() {
        this.clear();
        if (this.visible) {
            this.context.beginPath();
            if (this.shape === "circle") {
                this.context.arc(this.stimFigure.x, this.stimFigure.y, this.stimFigure.size, 0, 2 * Math.PI, false);
            } else {
                this.context.rect(this.stimFigure.x, this.stimFigure.y, this.stimFigure.size * 2, this.stimFigure.size * 2);
            }
            this.context.fillStyle = this.stimFigure.color;
            this.context.fill();
        }
    }

    toggle(visible) {
        this.visible = visible;
    }

    calculatePursuit() {
        let time = this.tick += this.step;
        let centerX = this.canvas.width / 2 - this.stimFigure.size / 2;
        let sinus = Math.sin(time * 2 * Math.PI / this.period);
        let sinusRound = sinus.toFixed(9);
        let sinusAbs = Math.abs(sinusRound);
        let nextX = this.amplitude * sinus + centerX;
        let center = this.canvas.width / 2 - this.circleSize / 2;
        this.stimFigure.x = nextX;
        if (sinusRound == -1) {
            this.counterMinus++;
        }
        if (sinusRound == 1) {
            this.counterPlus++;
        }
        if (sinusAbs == 0) {
            this.counterZero++;
            this.progress();
        }
        return sinusRound;
    }

    calculateSaccade() {
        let next = this.fixations[this.fixation];
        let center = this.canvas.width / 2 - this.circleSize / 2;
        let now = performance.now();
        switch (next) {
            case 0:
                this.stimFigure.x = center;
                break;
            case 1:
                this.stimFigure.x = center + this.amplitude;
                break;
            case -1:
                this.stimFigure.x = center - this.amplitude;
                break;
        }
        if (now - this.lastSaccadeTime >= this.period) {
            this.fixation++;
            this.lastSaccadeTime = now;
            this.progress();
        }
    }

    calculate() {
        let self = this;
        switch (self.movement) {
            case "pursuit":
                return self.calculatePursuit();
                break;

            case "saccade":
                return self.calculateSaccade();
                break;
        }
    }

    setCanvasSize() {
        if (!this.initialized) {
            return;
        }
        this.canvas.width = window.innerWidth;
        this.canvas.height = window.innerHeight;
        this.stimFigure.y = this.canvas.height / 2 - this.circleSize / 2;
    }

    animate() {
        let self = this;
        if (this.initialized) {
            if (self.running === true) {
                self.calculate();
            }
            self.draw();
            self.report();
        }
        requestAnimationFrame(() => self.animate());
    }

}