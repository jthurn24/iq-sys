
// This object is the view model for the patient form view
var PatientFormModel = function (sourceModel) {

    // This method takes properties defined on the source model and maps
    //  them to our view model, creating numerous observables in the process
    ko.mapping.fromJS(sourceModel, {}, this);

    this.showStatusChangeDate = ko.dependentObservable(
        function () {

            if (this.CurrentStatus() == null) {
                return true;
            }

            return this.CurrentStatus() != this.NewStatus();
        },
        this);



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

    this.roomChange = ko.dependentObservable(
        function () {

            if (this.Room() == null && this.CurrentRoom() != null) {
                return;
            }

            $('.roomchangedata').hide();

            if (this.CurrentRoom() == null) {
                $('.roomchangedata').hide();
                return;
            }

            if (this.Room() != null && this.Room() != this.CurrentRoom()) {
                $('.roomchangedata').show();
            }

        }, this);
}