
// This object configures all of the JavaScript functionality on the vaccine form view
var VaccineForm = function (clientViewModel) {

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
		// The following bindings contain some code, but this prevents us from having to create trivial dependent observables for these elements



		var viewModel = new VaccineFormModel(clientViewModel);
		ko.applyBindings(viewModel);

		viewModel.Floor(clientViewModel.Floor);
		viewModel.Wing(clientViewModel.Wing);
	});
}