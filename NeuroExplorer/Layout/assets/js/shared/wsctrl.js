class WebSocketController {

    constructor(config) {
        this.addr = "ws://127.0.0.1:7654" + config.endpoint;
        this.callbacks = config.callbacks;
        this.binary = (typeof config["binary"] !== "undefined"),
        this.instance = null;
        this.reconnector = null;
        this.start();
    }

    reconnect() {
        let self = this;
        clearTimeout(self.reconnector);
        self.reconnector = setTimeout(function () { self.start() }, 5 * 1000);
    }

    start() {
        let self = this;
        self.instance = new WebSocket(self.addr);
        if(self.binary){
            self.instance.binaryType = 'arraybuffer';
        }
        self.instance.onopen = function (evt) {
            if (typeof self.callbacks["onopen"] === "function") {
                self.callbacks["onopen"](evt);
            }
        };

        self.instance.onclose = function (evt) {
            self.reconnect();
            if (typeof self.callbacks["onclose"] === "function") {
                self.callbacks["onclose"](evt);
            }
        };

        self.instance.onmessage = function (evt) {
            if (typeof self.callbacks["onmessage"] === "function") {
                self.callbacks["onmessage"](evt);
            }
        };

        self.instance.onerror = function (evt) {
            self.reconnect();
            if (typeof self.callbacks["onerror"] === "function") {
                self.callbacks["onerror"](evt);
            }
        };
    }

    send(message){
        if (this.instance.readyState === this.instance.OPEN){
            this.instance.send(JSON.stringify(message));
        }
    }

}