var refreshDevices = function () {

    let devicesSelected = 0;
    $('.container-devices').html('');
    _.each(data.devices, function (device, index) {
        let newElement = _.template($("#template-device").html())(device);
        $('.container-devices').append(newElement);
        if (device.selected) {
            devicesSelected += 1;
        }
    });

    if (devicesSelected) {
        $('.checklist-devices-text').html('Selected items: ' + devicesSelected);
        $('.checklist-devices-status').html('<i class="ti-check text-success border-success"></i>');
    } else {
        $('.checklist-devices-text').html('Choose from below');
        $('.checklist-devices-status').html('<i class="ti-time text-warning border-warning"></i>');
    }

    let tabIndex = 0;
    app.state.charts = {};
    $('.preview-tabs-head').html('');
    $('.preview-tabs-body').html('');
    _.each(data.devices, function (device, index) {
        if (device.selected) {

            let newHeader = _.template($("#template-tab-header").html())(device);
            if (tabIndex === 0) {
                $(newHeader).appendTo('.preview-tabs-head').addClass('active');
            } else {
                $(newHeader).appendTo('.preview-tabs-head');
            }

            let newBody = _.template($("#template-tab-" + device.id).html())(device);
            if (tabIndex === 0) {
                $(newBody).appendTo('.preview-tabs-body').addClass('show active');
            } else {
                $(newBody).appendTo('.preview-tabs-body');
            }

            tabIndex++;
        }
    });

    window.dispatchEvent(new CustomEvent('SET_STATE', { 'detail': { selectedDevices: devicesSelected } }));

}

var updateDevice = function (deviceID, newProperties) {
    _.each(data.devices, function (device, index) {
        if (device.id === deviceID) {
            device = _.merge(device, newProperties)
        }
    });
}

var deselectAllDevices = function () {
    _.each(data.devices, function (device, index) {
        updateDevice(device.id, { selected: false })
    });
}

var devicesStatusHandler = function (receivedDevices) {
    let changes = 0;
    for (device in receivedDevices) {
        if (data.devices[device].status !== receivedDevices[device]) {
            updateDevice(device, { status: receivedDevices[device] });
            changes++;
        }
    }
    if (changes) {
        refreshDevices();
    }
}

var devicesLeapStart = function () {

    cont = document.createElement('div');
    cont.setAttribute('id', 'container-leap');
    cont.style.width = "640px";
    cont.style.height = "480px";
    cont.classList.add('hidden');
    document.body.appendChild(cont);

    Leap.loop()
        .use('boneHand', {
            targetEl: document.getElementById('container-leap'),
            arm: false,
            opacity: 0.8
        });

    var scene = Leap.loopController.plugins.boneHand.scene;
    var camera = Leap.loopController.plugins.boneHand.camera;
    var renderer = Leap.loopController.plugins.boneHand.renderer;
    var plane = new THREE.Mesh();

    plane.scale.set(2, 2, 2);
    plane.position.set(0, 200, -100);
    plane.receiveShadow = true;
    scene.add(plane);

    camera.lookAt(plane.position);

    var axisHelper = new THREE.AxisHelper(100);
    scene.add(axisHelper);

}

var deviceLeapCheckControls = function () {
    if (typeof app.state.leap["controls"] === 'undefined') {
        var camera = Leap.loopController.plugins.boneHand.camera;
        var renderer = Leap.loopController.plugins.boneHand.renderer;
        app.state.leap["controls"] = new THREE.OrbitControls(camera, renderer.domElement);
    }
}

var devicesDataReceiver = function () {
    let sockets = {};

    sockets["pulse"] = new WebSocketController({
        "endpoint": "/pulse",
        "callbacks": {
            "onmessage": function (evt) {

                if (typeof app.state.charts['chart-pleth'] === "undefined") {
                    return;
                }

                let msg = JSON.parse(evt.data);

                $('.pulse-pulse').text(msg.pulse);
                $('.pulse-spo2').text(msg.spo2);

                app.state.charts['chart-pleth'].series.append(moment().format('x'), msg.pulseWaveform);
                app.state.charts['chart-spo2'].series.append(moment().format('x'), msg.spo2);

            }
        }
    });

    sockets["eyetracker"] = new WebSocketController({
        "endpoint": "/eyetracker",
        "callbacks": {
            "onmessage": function (evt) {

                if (typeof app.state.charts['chart-eyetracker-x'] === "undefined") {
                    return;
                }

                let msg = JSON.parse(evt.data);

                app.state.charts['chart-eyetracker-x'].series.append(moment().format('x'), msg.gaze.smooth.x);
                app.state.charts['chart-eyetracker-y'].series.append(moment().format('x'), msg.gaze.smooth.y);

            }
        }
    });

    let eegChannels = ['raw'];
    sockets["eeg"] = new WebSocketController({
        "endpoint": "/eeg",
        "callbacks": {
            "onmessage": function (evt) {

                if (typeof app.state.charts['chart-eeg-' + eegChannels[0]] === "undefined") {
                    return;
                }

                let msg = JSON.parse(evt.data);
                let now = moment().format('x');

                for (let i = 0; i < eegChannels.length; i++){
                    if (msg['param'] === eegChannels[i]) {
                        app.state.charts['chart-eeg-' + eegChannels[i]].series.append(now, msg['value']);
                    }
                }

                if (msg['param'] === 'poorSignal') {
                    $('.eeg-status-icon').removeClass('text-success').removeClass('text-warning').removeClass('text-danger');
                    if(msg['value'] === 0){
                        $('.eeg-status-icon').addClass('text-success')
                    } else if (msg['value'] < 200) {
                        $('.eeg-status-icon').addClass('text-warning')
                    } else {
                        $('.eeg-status-icon').addClass('text-danger')
                    }
                }

            }
        }
    });

    sockets["mic"] = new WebSocketController({
        "endpoint": "/mic",
        "callbacks": {
            "onmessage": function (evt) {

                if (typeof app.state.charts['chart-mic'] === "undefined") {
                    return;
                }

                let msg = JSON.parse(evt.data);

                app.state.charts['chart-mic'].series.append(moment().format('x'), msg.peak);

            }
        }
    });

    let gsrConfig = {
        "buffer" : {
            "conductivity": new CircularBuffer(100),
            "resistance": new CircularBuffer(100),
            "change": new CircularBuffer(100)
        }
    }

    sockets["gsr"] = new WebSocketController({
        "endpoint": "/gsr",
        "callbacks": {
            "onmessage": function (evt) {

                if (typeof app.state.charts['chart-gsr-resistance'] === "undefined") {
                    return;
                }

                let msg = JSON.parse(evt.data);

                gsrConfig.buffer.conductivity.enq(msg.conductivity_uSiemens);
                gsrConfig.buffer.resistance.enq(msg.resistance_kOhm);
                gsrConfig.buffer.change.enq(msg.percentChange);

                $('.gsr-conductivity').text(math.mean(gsrConfig.buffer.conductivity.toarray()).toFixed(2));
                $('.gsr-resistance').text(math.mean(gsrConfig.buffer.resistance.toarray()).toFixed(2));
                $('.gsr-change').text(math.mean(gsrConfig.buffer.change.toarray()).toFixed(2));

                app.state.charts['chart-gsr-resistance'].series.append(moment().format('x'), msg.resistance_kOhm);

            }
        }
    });

    let dynamometerConfig = {
        "buffer": {
            "values": new CircularBuffer(100),
        }
    }

    sockets["dynamometer"] = new WebSocketController({
        "endpoint": "/dynamometer",
        "callbacks": {
            "onmessage": function (evt) {

                if (typeof app.state.charts['chart-dynamometer-value'] === "undefined") {
                    return;
                }

                let msg = JSON.parse(evt.data);

                dynamometerConfig.buffer.values.enq(msg.value);

                $('.dynamometer-current').text(msg.value.toFixed(2));
                $('.dynamometer-max').text(math.max(dynamometerConfig.buffer.values.toarray()).toFixed(2));

                app.state.charts['chart-dynamometer-value'].series.append(moment().format('x'), msg.value);

            }
        }
    });    

    devicesLeapStart();

    let openFaceConfig = {
        frameDiff: 0,
        frameDiffLimit: 10,
        overlayPoints: {}
    };

    let calculateEmotion = function(ausAll, ausRequired){
        let factors = [];
        let values = [];
        _.each(ausRequired, function (auRequiredName) {
                _.each(ausAll, function (auData) {
                    if(auData.au === auRequiredName){
                        factors.push(auData.presence);
                        values.push(auData.value);
                    }
                });
        });
        return (math.mean(factors) * math.mean(values) * 100) / 5;
    };

    sockets["openface"] = new WebSocketController({
        "endpoint": "/openface",
        "binary": true,
        "callbacks": {
            "onmessage": function (evt) {

                if (document.getElementById('canvas-openface') === null) {
                    return;
                }

                let msg = evt;
                let canvas = document.getElementById('canvas-openface');

                if (typeof msg.data == 'object') {
                    let arrayBuffer = msg.data;
                    let bytes = new Uint8Array(arrayBuffer);

                    app.jpegDecoder.parse(bytes);
                    let width = app.jpegDecoder.width;
                    let height = app.jpegDecoder.height;
                    let numComponents = app.jpegDecoder.numComponents;
                    let decoded = app.jpegDecoder.getData(width, height);

                    canvas.width = width;
                    canvas.height = height;
                    let ctx = canvas.getContext('2d');
                    let imageData = ctx.createImageData(width, height);
                    let imageBytes = imageData.data;
                    for (let i = 0, j = 0, ii = width * height * 4; i < ii;) {
                        imageBytes[i++] = decoded[j++];
                        imageBytes[i++] = numComponents === 3 ? decoded[j++] : decoded[j - 1];
                        imageBytes[i++] = numComponents === 3 ? decoded[j++] : decoded[j - 1];
                        imageBytes[i++] = 255;
                    }
                    ctx.putImageData(imageData, 0, 0);
                    openFaceConfig.frameDiff++;

                    if (openFaceConfig.frameDiff > openFaceConfig.frameDiffLimit) {
                        return;
                    }

                    ctx.strokeStyle = "rgba(255,255,255,.3)";
                    ctx.fillStyle = "rgba(255,255,255,.6)";
                    let pointsLength = openFaceConfig.overlayPoints.length;
                    for (let j = 0; j < pointsLength; j++) {
                        ctx.beginPath();
                        ctx.arc(openFaceConfig.overlayPoints[j].x, openFaceConfig.overlayPoints[j].y, 1, 0, 2 * Math.PI);
                        ctx.stroke();
                        let next = j + 1;
                        if (j == 16 || j == 21 || j == 26) {
                            continue;
                        }
                        if (j == 35) {
                            next = 30;
                        }
                        if (j == 41) {
                            next = 36;
                        }
                        if (j == 47) {
                            next = 42;
                        }
                        if (j == 67) {
                            next = 60;
                        }
                        if (next < pointsLength) {
                            ctx.lineTo(openFaceConfig.overlayPoints[next].x, openFaceConfig.overlayPoints[next].y);
                            ctx.stroke();
                        }
                        ctx.closePath();
                    }

                } else {
                    let obj = JSON.parse(msg.data);
                    openFaceConfig.overlayPoints = obj.overlayPoints;
                    openFaceConfig.frameDiff = 0;

                    let happiness = calculateEmotion(obj.actionUnits, ["AU06", "AU12"]);
                    let sadness = calculateEmotion(obj.actionUnits, ["AU01", "AU04", "AU15"]);
                    let surprise = calculateEmotion(obj.actionUnits, ["AU01", "AU02", "AU05", "AU26"]);
                    let fear = calculateEmotion(obj.actionUnits, [ "AU01", "AU02", "AU04", "AU05", "AU07", "AU20", "AU26"]);
                    let anger = calculateEmotion(obj.actionUnits, ["AU04", "AU05", "AU07", "AU23"]);
                    let disgust = calculateEmotion(obj.actionUnits, ["AU09", "AU15"]);

                    $('.emotion-happiness-progress').css('width', happiness + '%');
                    $('.emotion-sadness-progress').css('width', sadness + '%');
                    $('.emotion-surprise-progress').css('width', surprise + '%');
                    $('.emotion-fear-progress').css('width', fear + '%');
                    $('.emotion-anger-progress').css('width', anger + '%');
                    $('.emotion-disgust-progress').css('width', disgust + '%');

                }

            }
        }
    });

}();