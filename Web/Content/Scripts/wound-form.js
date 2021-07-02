
// This object configures all of the JavaScript functionality on the infection form view
var WoundForm = function (clientViewModel) {

    $(function () {

        var viewModel = new WoundFormModel(clientViewModel);
        ko.applyBindings(viewModel);

        lookupSite();
    });
}

function point_it(event) {

        pos_x = event.offsetX ? (event.offsetX) : event.pageX - document.getElementById("SiteImageCanvas").offsetLeft;
        pos_y = event.offsetY ? (event.offsetY) : event.pageY - document.getElementById("SiteImageCanvas").offsetTop;
        select_it(pos_x, pos_y);
}

function select_it(x, y) {

    var date = new Date();
    var link = SITE_CLICK_ACTION + '?x=' + x + '&y=' + y + "&nocache=" + date.getTime()
    $('#SiteImage').attr("src", link);
    $('#LocationX').val(x);
    $('#LocationY').val(y);

    lookupSite();
}

function lookupSite() {

    var x = $('#LocationX').val();
    var y = $('#LocationY').val();
    var date = new Date();

    if (x != '' && y != '') {

        var lookupLink = SITE_SITE_LOOKUP_ACTION + '?x=' + x + '&y=' + y + "&nocache=" + date.getTime();

        $.getJSON(lookupLink, function (data) {
            $(".SiteName").html(data['name']);
            $("#Site").val(data['id']);
        });
    }
}