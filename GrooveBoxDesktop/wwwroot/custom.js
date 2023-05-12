function initializeTooltips() {
    $('.truncated-title').hover(function () {
        var fullTitle = $(this).data('fulltitle');
        $(this).attr('title', fullTitle);
    });
}

var myModal = document.getElementById('myModal')
var myInput = document.getElementById('myInput')

myModal.addEventListener('shown.bs.modal', function () {
    myInput.focus()
})