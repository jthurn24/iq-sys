
// This object is the view model for the infection form view
var IncidentFormModel = function (sourceModel) {

    koHelper.ensureArray('Witnesses', sourceModel);
    koHelper.addMapping('Witnesses', 'WitnessModel', this);
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


    this.OcurredDateEnabled = ko.dependentObservable(
        function () {
            if (this.OcurredUnknown() == true) {
                return false;
            }
            return true;
        },
        this);

    this.roomSelectEnabled = ko.dependentObservable(
        function () {
            return this.Wing() > 0;
        },
        this);

    this.roomOptions = ko.dependentObservable(
        function () {
            var selectedWing = this.Wing();

            return ko.utils.arrayFilter(
                this.Rooms(),
                function (item) {
                    return item.Wing() == selectedWing;
                });
        },
        this);

    this.addWitness = function () {
        var data = { Name: '', Role: '', Removed: false, IncidentWitnessId: '', Statement: '' };
        this.Witnesses.unshift(new WitnessModel(data, this.Witnesses));
    }


}

var WitnessModel = function (sourceModel, parent) {
    koHelper.addChildArrayHelpers(this, parent);
    koHelper.mapJS(sourceModel, this);

    this.removeWitness = function () {
        this.Removed(true);
    };

}