(function($) {

    window.app = new Application('login');

    $('.btn-switch-page').click(function(e) {
        e.preventDefault();
        let pageName = $(this).data('page');
        app.setPage(pageName);
        app.setState({ experimentName: undefined, experimentId: "idle" });
    });

    $('body').on('change', '.check-select-device', function(e) {
        if ($(this).prop("checked")) {
            updateDevice($(this).data('device'), { selected: true });
        } else {
            updateDevice($(this).data('device'), { selected: false });
        }
        refreshDevices();
    });

    $('body').on('click', '.device-configure', function(e) {
        app.sendMessage("controller", $(this).data('action'), $(this).data('device'));
    });

    $('.start-examination').click(function(e) {
        e.preventDefault();
        let newExaminationID = createExamination(app.state.patientId, "Test Examination");
        $('.examination-id').html(newExaminationID);
        $('.examination-notes').html('');
        app.setPage('live-view');
        $('.preview-tabs-head').children('a').first().trigger('click');
        $('.container-stages').animate({ scrollTop: 0 }, "fast");
        app.propagateMetadata();
    });

    $('.form-login').submit(function(e) {
        e.preventDefault();
        let firstname = $('.login-firstname').val();
        let lastname = $('.login-lastname').val();
        app.setState({ researcher: { firstname: firstname, lastname: lastname } });
        $('.researcher-initials').text(firstname.charAt(0) + lastname.charAt(0));
        $('.researcher-name').text(firstname + ' ' + lastname);
        app.setPage('select-experiment');
    });

    $('.link-logout').click(function(e) {
        e.preventDefault();
        app.setPage('login');
    });

    $('.select-experiment').click(function(e) {
        e.preventDefault();
    });

    $('.select-patient').change(function(e) {
        deselectAllPatients();
        app.setState({ patientId: undefined });
        if ($(this).val()) {
            let patientId = $(this).val();
            updatePatient(patientId, { selected: true });
            app.setState({ patientId: patientId });
        }
        refreshPatients();
    });

    $('.input-new-patient-id').on("keydown", function(event) {
        if (event.which == 13) {
            $('.btn-add-patient').trigger('click');
        }
    });

    $('.btn-add-patient-modal').click(function(e) {
        e.preventDefault();
        $('#modal-new-patient').modal('show')
    });

    $('.btn-add-patient').click(function(e) {
        let newPatientID = $('.input-new-patient-id').val();
        if (newPatientID) {
            let status = addPatient({ id: newPatientID, selected: true, history: {} });
            if (status) {
                app.setState({ patientId: newPatientID });
                refreshPatients();
                $('#newPatientModal').trigger('click');
                $('.input-new-patient-id').val('');
                $('#modal-new-patient').modal('hide');
            }
        }
    });

    $('body').on('click', '.remove-note', function(e) {
        e.preventDefault();
        let patientId = $(this).data('patient');
        let examinationId = $(this).data('examination');
        let noteId = $(this).data('note');
        removeExaminationNote(patientId, examinationId, noteId);
        $(this).parentsUntil('.examination-notes').remove();
    });

    var addNote = function() {
        let noteText = $('.input-add-note').val();
        let newNoteId = addExaminationNote(app.state.patientId, app.state.examinationId, noteText);
        let newNoteData = {
            "date": now(),
            "noteId": newNoteId,
            "patientId": app.state.patientId,
            "examinationId": app.state.examinationId,
            "note": noteText
        };
        let newNote = _.template($("#template-note").html())(newNoteData);
        $(newNote).appendTo('.examination-notes');
        $('.input-add-note').val('');
    }

    $(".input-add-note").on("keydown", function(event) {
        if (event.which == 13) {
            addNote();
        }
    });

    $('body').on('click', '.add-note', function(e) {
        e.preventDefault();
        addNote();
    });

    $("body").on('change', '.edit-note', function(e) {
        e.preventDefault();
        editExaminationNote(app.state.patientId, app.state.examinationId, $(this).data('note'), $(this).val());
    });

    $('.btn-start-examination').click(function(e) {
        e.preventDefault();
        if (!app.state.patientId) {
            warn('Please, select the patient');
        } else if (!app.state.selectedDevices) {
            warn('Please, select at least one device');
        } else {
            $('#modal-confirmation').modal('show');
        }
    });

    $('.btn-experiment-finish').click(function(e) {
        $('#modal-confirmation-finish').modal('show');
    });

    $('.finish-examination').click(function(e) {
        e.preventDefault();
        if (app.state.isRecording) {
            app.sendMessage("controller", "record", { "trigger": "stop" });
        }
        app.setPage('select-experiment');
        app.setState({ experimentId: "idle" });
        deselectAllDevices();
    });

    $('body').on('click', '.btn-select-experiment', function(e) {
        e.preventDefault();
        let experimentId = $(this).data('experiment-id');
        let experimentName = $(this).data('experiment-name');
        $('.heading-experiment-name').html(experimentName);
        app.setState({ experimentName: experimentName, experimentId: experimentId });
        app.setPage('prepare');
        if (data.experiments[experimentId]['settings']['stages'] === 'auto') {
            $('.btn-experiment-start').removeClass('hidden');
            $('.btn-experiment-pause').removeClass('hidden');
        } else {
            $('.btn-experiment-start').addClass('hidden');
            $('.btn-experiment-pause').addClass('hidden');
        }
    });

    $('body').on('click', '.btn-experiment-start', function(e) {
        e.preventDefault();
        app.sendMessage("experiment", "start", null);
    });

    $('body').on('click', '.btn-experiment-pause', function(e) {
        e.preventDefault();
        app.sendMessage("experiment", "stop", null);
    });

    $('body').on('click', '.btn-experiment-reset', function(e) {
        e.preventDefault();
        app.sendMessage("experiment", "reset", null);
        resetStages();
    });

    $('body').on('click', '.btn-experiment-record', function(e) {
        e.preventDefault();
        let exDetails = {
            patientId: app.state.patientId,
            examinationId: app.state.examinationId,
            experimentId: app.state.experimentId,
            researcherFirstname: app.state.researcher.firstname,
            researcherLastname: app.state.researcher.lastname
        };
        if (app.state.isRecording) {
            app.sendMessage("controller", "record", { "trigger": "stop" });
        } else {
            app.sendMessage("controller", "record", { "trigger": "start", "details": exDetails });
        }
    });

    $('body').on('click', '.stage-select', function(e) {
        e.preventDefault();
        app.setStage($(this).data('id'));
    });

    $('body').on('click', '.btn-open-folder', function(e) {
        e.preventDefault();
        app.sendMessage("controller", "folder", $(this).data('name'));
    });

    $('body').on('shown.bs.tab', '.nav-tabs a', function(e) {
        let x = $(e.target); // active tab
        let y = $(e.relatedTarget); // previous tab

        if (x.data('device-id') === "leap") {
            deviceLeapCheckControls();
            $('#container-leap').removeClass('hidden');
            onResize();
        } else {
            $('#container-leap').addClass('hidden');
        }

        $('#tab-' + x.data('device-id')).find('.enchanted').each(function(index) {
            let name = $(this).data('name');
            let minVal = $(this).data('min');
            let maxVal = $(this).data('max');
            let canvas = $(this).find('canvas')[0];

            if (typeof app.state.charts[name] !== "undefined") {
                return;
            }

            let chartConfig = {
                minValue: minVal,
                maxValue: maxVal,
                grid: {
                    fillStyle: '#ffffff',
                    strokeStyle: 'rgba(0, 0, 0, 0.125)'
                },
                labels: {
                    fillStyle: '#212529'
                },
                timestampFormatter: SmoothieChart.timeFormatter,
                responsive: true,
                tooltip: true
            };

            app.state.charts[name] = {
                chart: new SmoothieChart(chartConfig),
                canvas: canvas,
                series: new TimeSeries()
            };

            app.state.charts[name].chart.addTimeSeries(app.state.charts[name].series, { lineWidth: 1.5, strokeStyle: '#17a2b8' });
            app.state.charts[name].chart.streamTo(app.state.charts[name].canvas, 0);

        });
    });

    var onResize = function() {
        if ($('#container-leap').length > 0) {
            $('#container-leap')
                .offset($('#placeholder-leap').offset())
                .width($('#placeholder-leap').width())
                .height($('#placeholder-leap').height());
        }
    }

    $(window).resize(function() {
        onResize();
    });

    var clockInterval = setInterval(() => {
        if (app.state.pageName === "live-view") {
            $('.text-current-time').text(moment().format("HH:mm:ss"));
        }
    }, 500);

    var patientsHistoryInterval = setInterval(() => {
        if (app.state.pageName === "patients") {
            getPatients();
        }
    }, 500);

    var devicesStatusInterval = setInterval(() => {
        if (app.state.pageName === "prepare") {
            app.sendMessage("controller", "status", "");
        }
    }, 500);

})(jQuery);