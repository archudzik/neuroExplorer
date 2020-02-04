var generateGuid = function () {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

var now = function(){
    return moment().format("YYYY-MM-DD HH:mm:ss");
}

var setState = function(obj){
    window.dispatchEvent(new CustomEvent('SET_STATE', { 'detail': obj }));
}

var warn = function (msg) {
    new Noty({
        type: 'warning',
        layout: 'topRight',
        theme: 'bootstrap-v4',
        timeout: 1500,
        text: msg
    }).show();
}

var cheer = function (msg) {
    new Noty({
        type: 'success',
        layout: 'topRight',
        theme: 'bootstrap-v4',
        timeout: 1500,
        text: msg
    }).show();
}