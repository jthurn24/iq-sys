
// This object configures all of the JavaScript functionality on the infection form view
var WoundAssessmentForm = function (clientViewModel) {

    $(function () {


        // Set up bindings for the select lists
        $("select#Floor").dataBind({
            value: "Floor",
            options: "Floors",
            optionsText: "'Text'",
            optionsValue: "'Value'",
            optionsCaption: "' '"
        });

        $("select#Wing").dataBind({
            value: "Wing",
            options: "wingOptions",
            optionsText: "'Text'",
            optionsValue: "'Value'",
            optionsCaption: "' '",
            enable: "wingSelectEnabled"
        });

        $("select#Room").dataBind({
            value: "Room",
            options: "roomOptions",
            optionsText: "'Text'",
            optionsValue: "'Value'",
            optionsCaption: "' '",
            enable: "roomSelectEnabled"
        });

        $('#CalcPushButton').click(function () { wound_calc_push_score(); });

        var viewModel = new WoundAssessmentFormModel(clientViewModel);
        ko.applyBindings(viewModel);

        viewModel.Floor(clientViewModel.Floor);
        viewModel.Wing(clientViewModel.Wing);

    });
}

function wound_calc_push_score() {

    var score = 0;
    var log = 'Calculation results: \n';

    var l = $('#Lcm').val();
    var w = $('#Wcm').val();

    if (l == '' || w == '') {
        alert('Unable to calculate PUSH score: You must specify both length and width');
        return;
    }

    var lxw = l * w;

    if (lxw < 0.3) {
        score = score + 1;
        log += 'LxW: ' + lxw + ' (PUSH: 1) \n';
    }
    else if (lxw <= 0.6) {
        score = score + 2;
        log += 'LxW: ' + lxw + ' (PUSH: 2) \n';
    }
    else if (lxw <= 1) {
        score = score + 3;
        log += 'LxW: ' + lxw + ' (PUSH: 3) \n';
    }
    else if (lxw <= 2) {
        score = score + 4;
        log += 'LxW: ' + lxw + ' (PUSH: 4) \n';
    }
    else if (lxw <= 3) {
        score = score + 5;
        log += 'LxW: ' + lxw + ' (PUSH: 5) \n';
    }
    else if (lxw <= 4) {
        score = score + 6;
        log += 'LxW: ' + lxw + ' (PUSH: 6) \n';
    }
    else if (lxw <= 8) {
        score = score + 7;
        log += 'LxW: ' + lxw + ' (PUSH: 7) \n';
    }
    else if (lxw <= 12) {
        score = score + 8;
        log += 'LxW: ' + lxw + ' (PUSH: 8) \n';
    }
    else if (lxw <= 24) {
        score = score + 9;
        log += 'LxW: ' + lxw + ' (PUSH: 9) \n';
    }
    else
    {
        score = score + 10;
        log += 'LxW: ' + lxw + ' (PUSH: 10) \n';
    }

    var eType = $('#Exudate').val();

    if (eType == 'None') {
        log += 'Exudate: ' + eType + ' (PUSH: 0) \n';
    }
    else if (eType == 'Scant') {
        score = score + 1;
        log += 'Exudate: ' + eType + ' (PUSH: 1) \n';
    }
    else if (eType == 'Moderate') {
        score = score + 2;
        log += 'Exudate: ' + eType + ' (PUSH: 2) \n';
    }
    else if (eType == 'Heavy') {
        score = score + 3;
        log += 'Exudate: ' + eType + ' (PUSH: 3) \n';
    }
    else if (eType == 'Sanguineous') {
        score = score + 3;
        log += 'Exudate: ' + eType + ' (PUSH: 3) \n';
    }

    var epith = parseInt($('#WoundBedEpithelial').val()) || 0;
    var granulation = parseInt($('#WoundBedGranulation').val()) || 0;
    var slough = parseInt($('#WoundBedSlough').val()) || 0;
    var necrosis = parseInt($('#WoundBedNecrosis').val()) || 0;


    if (necrosis > 0) {
        score = score + 4;
        log += 'Tissue Type: Necrosis (PUSH: 4) \n';
    }
    else if (slough > 0) {
        score = score + 3;
        log += 'Tissue Type: Slough (PUSH: 3) \n';
    }
    else if (granulation > 0) {
        score = score + 2;
        log += 'Tissue Type: Granulation (PUSH: 2) \n';
    }
    else if (epith > 0 ) {
        score = score + 1;
        log += 'Tissue Type: Epithelial (PUSH: 1) \n';
    }
    else {
        log += 'Tissue Type: Closed (PUSH: 0) \n';
    }


    log += '\n Total:' + score;

    alert(log);

    $('#PushScore').val(score);
}