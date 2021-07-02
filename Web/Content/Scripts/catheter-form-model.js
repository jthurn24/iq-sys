
// This object is the view model for the infection form view
var CatheterFormModel = function (sourceModel) {


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


}
