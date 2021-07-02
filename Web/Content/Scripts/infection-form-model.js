
// This object is the view model for the infection form view
var InfectionFormModel = function (sourceModel) {

    koHelper.ensureArray('LabResults', sourceModel);
    koHelper.addMapping('LabResults', 'LabResultModel', this);
    koHelper.ensureArray('TreatmentTypes', sourceModel);
    koHelper.addMapping('TreatmentTypes', 'TreatmentTypeModel', this);
    koHelper.ensureArray('InfectionNotes', sourceModel);
    koHelper.addMapping('InfectionNotes', 'NoteModel', this);
    koHelper.mapJS(sourceModel, this);

    // Set up dependent observables
    this.wingSelectEnabled = ko.dependentObservable(
        function () {
            return this.Floor() > 0;
        },
        this);

    this.wingOptions = ko.dependentObservable(
        function () {
            this.Wing(null); // Must manually set this
            var selectedFloor = this.Floor();

            if (selectedFloor <= 0)
                return [];

            return ko.utils.arrayFilter(
                this.Wings(),
                function (item) {
                    return item.Floor() == selectedFloor;
                });
        },
        this);

    this.roomSelectEnabled = ko.dependentObservable(
        function () {
            return this.Wing() > 0;
        },
        this);

    this.roomOptions = ko.observable();


    // Creating new observables for infection type and site since the mapped ones are for integer values only

    this.hideCriteria = ko.observable();

    this.hiddenCriteriaMessage = ko.observable();

    this.selectedInfectionType = ko.observable();

    this.selectedInfectionSite = ko.observable();

    this.selectedLabResultType = ko.observable();

    this.selectedNote = ko.observable('');

    this.infectionSiteDetailsDisplay = ko.observable(false);

    this.infectionSiteDetailsItems = ko.observableArray();

    this.infectionSiteDetailsDescription = ko.observable('');

    this.infectionSelectedSiteDetail = ko.observable();

    this.afterTreatmentTypeRender = function (e, d) {
        d.TreatmentNameCombo = $(e).find(".treatment-list");
        $(e).find(".treatment-list").autocomplete({
            source: d.TreatmentNames()
        });

        d.DurationCombo = $(e).find(".duration-list");
        $(e).find(".duration-list").autocomplete({
            source: ['Daily', 'Once', '2 days', '3 days', '4 days', '5 days', '6 days', '7 days', '10 days', '2 weeks', 'On-going', 'PRN']
        });

        d.FrequencyCombo = $(e).find(".frequency-list");
        $(e).find(".frequency-list").autocomplete({
            source: ['BID', 'Daily', 'q 1 hour', 'q 2 hour', 'q 3 hour', 'q 4 hour', 'q 5 hour', 'q 6 hour', 'q 7 hour', 'q 8 hour', 'q 9 hour', 'q 10 hour', 'q 11 hour', 'q 12 hour', 'QID', 'TID']
        });

        d.DeliveryCombo = $(e).find(".delivery-list");
        $(e).find(".delivery-list").autocomplete({
            source: ['Ear Drops', 'Eye Drops', 'IM', 'IV', 'Oral', 'Per Tube', 'Topical']
        });

        d.InstructionCombo = $(e).find(".instructions-list");
        $(e).find(".instructions-list").autocomplete({
            source: ['After Meals', 'Avoid Alcohol', 'Before Meals', 'Check Expiration Date', 'Crushed', 'Do Not Crush', 'Empty Stomach', 'Refrigerate', 'Shake Well Before Use', 'With Food']
        });

        d.AdministeredOnBox = $(e).find(".treatment-date");
        d.AdministeredOnBox.datepicker({
            changeYear: true,
            gotoCurrent: true
        });

        trapEnter($(e).find('input'));
    }

    this.resultOptions = ko.observableArray();
    this.selectedResultOption = ko.observable();

    this.selectedLabResultType.subscribe(function (newValue) {

        this.resultOptions([]);

        if (newValue != null) {
            $.ajax({
                url: '/Infection/GetApplicableLabResults/?testId=' + newValue.Id(),
                context: this,
                cache: false,
                success: function (data) {
                    if (data != null) {
                        var newArray = [];
                        for (i = 0; i < data.length; i++) {
                            newArray.push(ko.mapping.fromJS(data[i]));
                        }
                        this.resultOptions(newArray);
                        this.resultOptions.valueHasMutated();
                    }
                }
            });
        }
    }, this);

    this.selectedInfectionSite.subscribe(function (newValue) {

        this.infectionSiteDetailsItems([]);
        this.infectionSiteDetailsDisplay(false);
        if (newValue != null) {

            /* Load Site Details */
            $.ajax({
                url: '/Infection/GetSiteDetails/?id=' + newValue.Value() + '&patient=' + this.Patient() + '&infectionId=' + this.InfectionVerificationId(),
                context: this,
                cache: false,
                success: function (data) {

                    this.hideCriteria(data['hideCriteria']);
                    this.hiddenCriteriaMessage(data['hiddenCriteriaMessage']);

                    if (data['items'] != null) {
                        var newArray = [];
                        for (i = 0; i < data['items'].length; i++) {
                            newArray.push(ko.mapping.fromJS(data['items'][i]));
                        }
                        this.infectionSiteDetailsItems(newArray);
                        this.infectionSiteDetailsItems.valueHasMutated();
                        if (data['items'].length > 0) {
                            this.infectionSiteDetailsDisplay(true);
                        }
                        this.infectionSiteDetailsDescription(data["description"]);

                        if (this.SelectedSiteDetail() != null) {
                            this.infectionSelectedSiteDetail(this.SelectedSiteDetail());
                            this.SelectedSiteDetail(null);
                        }

                    }
                }
            });

        }
    }, this);


    this.Wing.subscribe(function (newValue) {

        var selectedWing = newValue;
        this.roomOptions([]);

        var filtered = [];

        for (i = 0; i < this.Rooms().length; i++) {
            if (this.Rooms()[i].Wing() == selectedWing) {
                filtered.push(this.Rooms()[i]);
            }
        }

        this.roomOptions(filtered);

        this.roomOptions.valueHasMutated();

    }, this);

    this.addNote = function () {

        var data = { CreatedOn: '', CreatedBy: '', Removed: false, InfectionNoteId: '', Note: this.selectedNote() };
        this.InfectionNotes.unshift(new NoteModel(data, this.InfectionNotes));
        this.selectedNote('');
    }

    this.addLabResult = function () {
        if (this.selectedLabResultType() == null || this.selectedLabResultType() == '') {
            alert("You must select a result type");
            return;
        }

        if (this.selectedResultOption() == null || this.selectedResultOption() == '') {
            alert("You must select a result");
            return;
        }


        $('#addLabTestPrompt').show();

        $.ajax({
            url: '/Infection/CreateLabResult/?testId=' + this.selectedLabResultType().Id() + '&competedOn=' + $('#LabResultDateCompleted').val() + '&resultId=' + this.selectedResultOption().Id(),
            context: this,
            cache: false,
            success: function (data) {
                var result = new LabResultModel(data, this.LabResults);
                this.LabResults.unshift(result);

                this.selectedLabResultType(null);
                $('#LabResultDateCompleted').val('');
                this.selectedResultOption(null);

                $('#addLabTestPrompt').fadeOut();
            }
        });

    }

    this.selectInfectionType = function (value) {
        this.selectedInfectionType(
            ko.utils.arrayFirst(
                this.InfectionTypes(),
                function (item) {
                    return item.Value() == value;
                }));
    }

    this.selectInfectionSite = function (value) {
        this.selectedInfectionSite(
            ko.utils.arrayFirst(
                this.InfectionSites(),
                function (item) {
                    return item.Value() == value;
                }));
    }

    // Set up dependent observables

    this.infectionSiteOptions = ko.dependentObservable(
        function () {
            this.selectedInfectionSite(null);
            var $selectedInfectionType = this.selectedInfectionType();

            if ($selectedInfectionType == null)
                return [];

            return ko.utils.arrayFilter(
                this.InfectionSites(),
                function (item) {
                    return item.InfectionType() == $selectedInfectionType.Value();
                });
        },
        this);


    this.ruleSetComments = ko.dependentObservable(
        function () {
            var $selectedInfectionSite = this.selectedInfectionSite();

            if ($selectedInfectionSite == null)
                return null;

            return $selectedInfectionSite.RuleSet.Comments();
        },
        this);

    this.criteriaRuleSet = ko.dependentObservable(
        function () {
            var $selectedInfectionSite = this.selectedInfectionSite();

            if ($selectedInfectionSite == null)
                return null;

            return $selectedInfectionSite.RuleSet;
        },
        this);



    this.rulesSatisfied = ko.dependentObservable(
        function () {
            var $criteriaRuleSet = this.criteriaRuleSet();
            var $SelectedCriteria = this.SelectedCriteria();

            if ($criteriaRuleSet != null) {

                var matches = this.countSatisfiedRules($criteriaRuleSet);
                return matches >= $criteriaRuleSet.MinimumRules();
            }

            return false;
        },
        this);


    this.countSatisfiedRules = function (ruleSet) {

        var satisfiedRuleCount = 0;
        var model = this;

        /* Eval rules */
        ko.utils.arrayForEach(
                    ruleSet.Rules(),
                    function (rule) {
                        var matchingCriteriaCount = 0;

                        ko.utils.arrayForEach(
                            rule.Criteria(),
                            function (criteria) {
                                var match = ko.utils.arrayFirst(
                                    model.SelectedCriteria(),
                                    function (selected) {
                                        return selected == criteria.Id();
                                    }, this);

                                if (match != null)
                                    matchingCriteriaCount += 1;
                            });

                        if (matchingCriteriaCount >= rule.MinimumCriteria())
                            satisfiedRuleCount += 1;
                    });

        /* Eval child rulesets */
        ko.utils.arrayForEach(
                    ruleSet.RuleSets(),
                    function (childRuleSet) {

                        var matchingRuleCount = model.countSatisfiedRules(childRuleSet);

                        if (matchingRuleCount >= childRuleSet.MinimumRules()) {
                            satisfiedRuleCount += 1;
                        }

                    });

        return satisfiedRuleCount;
    }

    this.rulesSatisfied.subscribe(function (newValue) {

        if (newValue != null) {
            if (newValue) {
                $("#Classification").css("color", "#5e5843");
                $("#Classification").css("background-color", "#ffffcc");
                $("#Classification").css("font-weight", "bold");
            }
            else {
                $("#Classification").css("color", "white");
                $("#Classification").css("background-color", "#50616c");
                $("#Classification").css("font-weight", "bold");
            }
        }

    }, this);

    this.classificationOptions = ko.dependentObservable(
        function () {

            if (this.rulesSatisfied()) {
                return this.SatisfiedRuleClassifications();
            }
            else {
                return this.UnSatisfiedRuleClassifications();
            }


        }, this);


    this.classificationWarning = ko.dependentObservable(
        function () {

            var classification = this.Classification();
            var option = ko.utils.arrayFilter(
                this.classificationOptions(),
                function (item) {
                    return item.Value() == classification;
                });

            if (option === undefined || option.length < 1) {
                return '';
            }

            return option[0].Warning();

        }, this);

  
}

var LabResultModel = function (sourceModel, parent) {


    koHelper.addChildArrayHelpers(this, parent);

    koHelper.ensureArray('Pathogens', sourceModel);
    koHelper.addMapping('Pathogens', 'PathogenModel', this);
    koHelper.addMapping('PathogenOptions', 'PathogenOptionModel', this);
    koHelper.ensureArray('PathogenOptions', sourceModel);
    koHelper.addMapping('PathogenQuantityOptions', 'PathogenQuantityOptionModel', this);
    koHelper.ensureArray('PathogenQuantityOptions', sourceModel);

    koHelper.mapJS(sourceModel, this);

    this.selectedPathogen = ko.observable();
    this.selectedPathogenQuantity = ko.observable();

    this.removeLabResult = function () {
        this.Removed(true);
    }

    this.addPathogen = function () {

        if (this.showPathogenQuantities() && this.selectedPathogenQuantity() == null) {
            alert('You must select a pathogen quantity');
            return;
        }

        if (this.selectedPathogen().Id() < 0) {
            alert('You must select a pathogen');
            return;
        }

        var data = { PathogenName: this.selectedPathogen().Name(), PathogenId: this.selectedPathogen().Id(), Removed: false, Id: '' };

        if (this.selectedPathogenQuantity() == null) {
            data.PathogenQuantityName = '';
            data.PathogenQuantityId = '';
        }
        else {
            data.PathogenQuantityName = this.selectedPathogenQuantity().Name();
            data.PathogenQuantityId = this.selectedPathogenQuantity().Id();
        }

        this.Pathogens.push(new PathogenModel(data, this.Pathogens));

        this.selectedPathogen(this.PathogenOptions()[0]);
    }

    this.showPathogens = ko.dependentObservable(
        function () {
            if (this.PathogenOptions().length > 0) {
                return true;
            }

            return false;
        },
        this);

    this.showPathogenQuantities = ko.dependentObservable(
        function () {
            if (this.PathogenQuantityOptions().length > 0) {
                return true;
            }

            return false;
        },
        this);
}


var PathogenModel = function (sourceModel, parent) {

    koHelper.addChildArrayHelpers(this, parent);
    koHelper.mapJS(sourceModel, this);


    this.removePathogen = function () {
        this.Removed(true);
    }

}

var PathogenOptionModel = function (sourceModel, parent) {
    koHelper.addChildArrayHelpers(this, parent);
    koHelper.mapJS(sourceModel, this);
}

var PathogenQuantityOptionModel = function (sourceModel, parent) {
    koHelper.addChildArrayHelpers(this, parent);
    koHelper.mapJS(sourceModel, this);
}

var TreatmentTypeModel = function (sourceModel, parent) {
    koHelper.addMapping('Treatments', 'TreatmentModel', this);
    koHelper.ensureArray('Treatments', sourceModel);
    koHelper.addChildArrayHelpers(this, parent);
    koHelper.mapJS(sourceModel, this);

    this.SelectedDosage = ko.observable('');
    this.SelectedUnits = ko.observable('');
    this.SelectedMDName = ko.observable(DEFAULT_MD);
    this.TreatmentNameCombo = null;
    this.FrequencyCombo = null;
    this.DurationCombo = null;
    this.DeliveryCombo = null;
    this.InstructionCombo = null;
    this.AdministeredOnBox = null;



    this.addTreatment = function () {

        if ($(this.TreatmentNameCombo).val() == '') {
            alert('Treatment name is required');
            return;
        }

        var data = {
            TreatmentName: $(this.TreatmentNameCombo).val(),
            Dosage: this.SelectedDosage(),
            Frequency: $(this.FrequencyCombo).val(),
            DeliveryMethod: $(this.DeliveryCombo).val(),
            SpecialInstructions: $(this.InstructionCombo).val(),
            Units: this.SelectedUnits(),
            Duration: $(this.DurationCombo).val(),
            AdministeredOn: $(this.AdministeredOnBox).val(),
            MDName: this.SelectedMDName(),
            Removed: false,
            DiscontinuedOn: '',
            Id: ''
        };

        $(this.TreatmentNameCombo).val('');
        this.SelectedDosage('');
        $(this.FrequencyCombo).val('')
        $(this.DeliveryCombo).val('');
        $(this.InstructionCombo).val('');
        this.SelectedUnits('');
        $(this.DurationCombo).val('');
        $(this.AdministeredOnBox).val('');
        this.SelectedMDName(DEFAULT_MD);

        this.Treatments.push(new TreatmentModel(data, this.Treatments));

    };


    this.afterTreatmentRender = function (e, d) {

        var dBox = $(e).find(".discontinued-date");
        dBox.datepicker({
            changeYear: true,
            gotoCurrent: true
        });

    }

}

var TreatmentModel = function (sourceModel, parent) {
    koHelper.addChildArrayHelpers(this, parent);
    koHelper.mapJS(sourceModel, this);

    this.EditMode = ko.observable(false);

    this.removeTreatment = function () {
        this.Removed(true);
    };

    this.editTreatment = function () {
        this.EditMode(true);
    };

}

var NoteModel = function (sourceModel, parent) {
    koHelper.addChildArrayHelpers(this, parent);
    koHelper.mapJS(sourceModel, this);

    this.EditMode = ko.observable(false);

    this.removeNote = function () {
        this.Removed(true);
    };

    this.editNote = function () {
        this.EditMode(true);
    };

}

