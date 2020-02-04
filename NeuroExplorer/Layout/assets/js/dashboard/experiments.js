var refreshExperiments = function () {

    $('.container-experiments').html('');
    _.each(data.experiments, function (experiment, index) {
        let newElement = _.template($("#template-experiment").html())(experiment);
        $('.container-experiments').append(newElement);
    });

}