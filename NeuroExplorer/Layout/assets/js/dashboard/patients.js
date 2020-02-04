var getPatients = function (dataReceived) {
    if (typeof dataReceived === "undefined") {
        app.sendMessage("controller", "patients", {});
    } else {
        if (_.isEqual(data.patients, dataReceived) == false) {
            data.patients = dataReceived;
            refreshPatients();
        }
    }
}

var refreshPatients = function () {
    let patientSelected = 0;
    let thereIsHistory = false;
    $('.select-patient').html('<option></option>');
    $('.table-patients-index').html('');

    _.each(data.patients, function (patient, index) {
        let newElement = $('<option/>');
        newElement.data('tokens', patient.id).val(patient.id).text(patient.id);
        if (patient.selected) {
            newElement.attr('selected', 'selected');
            patientSelected += 1;
            $('.text-current-patient').html(patient.id);
        }
        if (patient.history) {
            _.each(patient.history, function (history) {
                thereIsHistory = true;
                $('.table-patients-index').append('<tr><td class="text-center"><button class="btn btn-outline-info btn-sm btn-open-folder" data-name="' + history.examinationId + '"><i class="fa fa-folder-open-o"></i></button></td><td>' + history.patientId + '<td>' + history.experimentName + '</td><td>' + history.date + '</td></tr>');
            });
        }
        $('.select-patient').append(newElement);
    });

    if (patientSelected) {
        $('.checklist-patient-status').html('<i class="ti-check text-success border-success"></i>');
    } else {
        $('.checklist-patient-status').html('<i class="ti-time text-warning border-warning"></i>');
    }

    if (!thereIsHistory) {
        $('.table-patients-index').html('<tr><td class="text-center">- empty -</td><td class="text-center">- empty -</td><td class="text-center">- empty -</td><td class="text-center">- empty -</td></tr>');
    }

    if (!$.fn.dataTable.isDataTable('.table-data-table')) {
        $('.table-data-table').DataTable({
            "order": [[3, "desc"]]
        });
    }
}

var updatePatient = function (patientID, newProperties) {
    _.each(data.patients, function (patient, index) {
        if (patient.id === patientID) {
            patient = _.merge(patient, newProperties)
        }
    });
}

var deselectAllPatients = function () {
    _.each(data.patients, function (patient, index) {
        updatePatient(patient.id, { selected: false })
    });
}

var addPatient = function (patientData) {
    if (typeof data.patients[patientData.id] !== "undefined") {
        warn('Patient <strong>' + patientData.id + '</strong> already exists');
        return false;
    }
    data.patients[patientData.id] = patientData;
    cheer('Patient <strong>' + patientData.id + '</strong> saved');
    return true;
}

var selectedPatient = function () {
    var selPatient = undefined;
    _.each(data.patients, function (patient, index) {
        if (patient.selected === true) {
            selPatient = patient;
        }
    });
    return selPatient;
}