
// This object configures all of the JavaScript functionality on the infection form view
var EmployeeInfectionForm = function (clientViewModel) {

    $(function () {

        // Set up bindings for the criteria-related select lists

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
            optionsCaption: "' '"
        });
      
        var viewModel = new EmployeeInfectionFormModel(clientViewModel);
        ko.applyBindings(viewModel);

        // For some reason, the call to ko.applyBindings overwrites these mapped values. 

        viewModel.Floor(clientViewModel.Floor);
        viewModel.Wing(clientViewModel.Wing);

    });
}