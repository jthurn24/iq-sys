
// This object configures all of the JavaScript functionality on the patient form view
var PatientForm = function (clientViewModel) {

    $(function () {

        // Set up bindings for the select lists
        $("#Floor").dataBind({
            value: "Floor",
            options: "Floors",
            optionsText: "'Text'",
            optionsValue: "'Value'",
            optionsCaption: "' '"
        });

        $("#Wing").dataBind({
            value: "Wing",
            options: "wingOptions",
            optionsText: "'Text'",
            optionsValue: "'Value'",
            optionsCaption: "' '",
            enable: "wingSelectEnabled"
        });

        $("#Room").dataBind({
            value: "Room",
            options: "roomOptions",
            optionsText: "'Text'",
            optionsValue: "'Value'",
            optionsCaption: "' '",
            enable: "roomSelectEnabled",
        });

        $("#CurrentRoom").dataBind({
            value: "CurrentRoom"
        });

        $("#NewStatus").dataBind({
            value: "NewStatus"
        });

        $("#CurrentStatus").dataBind({
            value: "CurrentStatus"
        });


        
        var viewModel = new PatientFormModel(clientViewModel);
        ko.applyBindings(viewModel);

        // For some reason, the call to ko.applyBindings overwrites these mapped values.

        viewModel.Floor(clientViewModel.Floor);
        viewModel.Wing(clientViewModel.Wing);
    });
}