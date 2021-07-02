    


function doSearch() {
    var searchString = $('#SearchBox').val();
    $('#TestList').html('Loading.....');
    $('#TestList').load(searchUrl + '?searchFor=' + searchString);
}

function doStartsWith(letter)
{
    $('#TestList').html('Loading.....');
    $('#TestList').load(searchUrl + '?startsWith=' + letter);
}


function selectDrug(id) {
    $('#SectionView').html('Loading.....');
    $('#SectionView').load(detailsUrl + '?id=' + id, function() {
        $('.section').first().show();
    });
}

function selectSection() {
    var id = $('.section_selector').first().val();
    $('.section').hide();
    $('#section_' + id).show();
}

$(function () {
    doStartsWith('A');
});