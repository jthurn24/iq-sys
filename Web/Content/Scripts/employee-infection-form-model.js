
// This object is the view model for the infection form view
var EmployeeInfectionFormModel = function (sourceModel) {

    koHelper.mapJS(sourceModel, this);

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
}
