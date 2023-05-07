function initializeTooltips() {
    $('.truncated-title').hover(function () {
        var fullTitle = $(this).data('fulltitle');
        $(this).attr('title', fullTitle);
    });
}