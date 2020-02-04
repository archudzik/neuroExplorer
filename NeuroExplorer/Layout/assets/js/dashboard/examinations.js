var createExamination = function (patientId, examinationName) {
    let newGuid = generateGuid();
    let newExamination = {
        "id": newGuid,
        "name": examinationName,
        "date": now(),
        "notes": {}
    };
    data.patients[patientId]['history'][newGuid] = newExamination;
    app.setState({ examinationId: newGuid });
    app.propagateMetadata();
    return newGuid;
}

var addExaminationNote = function (patientId, examinationId, note) {
    let newGuid = generateGuid();
    let newExaminationNote = {
        "id": newGuid,
        "note": note,
        "date": now()
    };
    data.patients[patientId]['history'][examinationId]['notes'][newGuid] = newExaminationNote;
    app.propagateMetadata();
    return newGuid;
}

var editExaminationNote = function (patientId, examinationId, noteId, note) {
    data.patients[patientId]['history'][examinationId]['notes'][noteId]['note'] = note;
    app.propagateMetadata();
}

var removeExaminationNote = function (patientId, examinationId, noteId) {
    delete data.patients[patientId]['history'][examinationId]['notes'][noteId];
    app.propagateMetadata();
}

var examinationRecordingHandler = function (status) {
    if (status) {
        $('.btn-experiment-record').removeClass('btn-outline-danger').addClass('btn-danger');
        $('.icon-record-status').addClass('recording');
        $('.text-record-status').text('Recording...');
    } else {
        $('.btn-experiment-record').removeClass('btn-danger').addClass('btn-outline-danger');
        $('.icon-record-status').removeClass('recording');
        $('.text-record-status').text('Record');
    }
    app.setState({ isRecording: status });
}