function initializeTooltips() {
    $('.truncated-title').hover(function () {
        var fullTitle = $(this).data('fulltitle');
        $(this).attr('title', fullTitle);
    });
}

myModal.addEventListener('shown.bs.modal', function () {
    myInput.focus()
})

window.closeModal = function (modalId) {
    $('#' + modalId).modal('hide');
}