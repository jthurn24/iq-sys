
// This object configures all of the JavaScript functionality on the infection form view
var InfectionForm = function (clientViewModel, criteriaDisabled) {

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

        if (!criteriaDisabled) {

            $("select#InfectionTypeSelect").dataBind({
                value: "selectedInfectionType",
                options: "InfectionTypes",
                optionsText: "'Text'",
                optionsCaption: "' '"
            });

            $("select#InfectionSiteSelect").dataBind({
                value: "selectedInfectionSite",
                options: "infectionSiteOptions",
                optionsText: "'Text'",
                optionsCaption: "' '"
            });

            $("#Classification").dataBind({
                value: "Classification",
                options: "classificationOptions",
                optionsText: "'Text'",
                optionsValue: "'Value'"
            });

            $("#RuleSetCriteria").dataBind({ template: "'ruleSetCriteriaTemplate'", visible: 'hideCriteria()==false' });
            $("#RuleSetComments").dataBind({ html: "ruleSetComments" });

            // The following bindings contain some code, but this prevents us from having to create trivial dependent observables for these elements

            $("input#InfectionType").dataBind({ value: "selectedInfectionType() == null ? '' : selectedInfectionType().Value()" });
            $("input#InfectionSite").dataBind({ value: "selectedInfectionSite() == null ? '' : selectedInfectionSite().Value()" });
            $("#InfectionTypeName").dataBind({ text: "selectedInfectionType() == null ? '?' : selectedInfectionType().Text()" });
            $("#InfectionSiteName").dataBind({ text: "selectedInfectionSite() == null ? '?' : selectedInfectionSite().Text()" });
        }



        $("select#LabResultType").dataBind({
            value: "selectedLabResultType",
            options: "LabTestTypes",
            optionsText: "'Name'",
            optionsCaption: "' '"
        });


        $("select#LabResultOption").dataBind({
            value: "selectedResultOption",
            options: "resultOptions",
            optionsText: "'Name'",
            optionsCaption: "' '"
        });


        //$("#SiteDetails").dataBind({ template: "'siteDetailsTemplate'" });



        // Bind models

        var viewModel = new InfectionFormModel(clientViewModel);
        ko.applyBindings(viewModel);

        // For some reason, the call to ko.applyBindings overwrites these mapped values. 

        if (!criteriaDisabled) {
            viewModel.selectInfectionType(clientViewModel.SelectedInfectionType);
            viewModel.selectInfectionSite(clientViewModel.SelectedInfectionSite);
            viewModel.Classification(clientViewModel.Classification);
        }

        viewModel.SelectedPrecautions(clientViewModel.SelectedPrecautions);

        viewModel.Floor(clientViewModel.Floor);
        viewModel.Wing(clientViewModel.Wing);


    });
}