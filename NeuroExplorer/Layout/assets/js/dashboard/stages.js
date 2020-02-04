var refreshStages = function (stages) {
    $('.container-stages').html('');
    _.each(stages, function (stage, index) {
        let newElement = _.template($("#template-stage").html())(stage);
        $('.container-stages').append(newElement);
    });
}

var updateStage = function (stage) {
    $('.stage-status:not(.stage-status-' + stage.id + ')').removeClass('text-success');
    $('.stage-status-' + stage.id).addClass('text-success');
    $('.stage-progress-' + stage.id).css('width', stage.data + '%');
}

var resetStages = function(){
    $('.stage-status').removeClass('text-success');
    $('.stage-progress').css('width', '0%');    
}