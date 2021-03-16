export function initialiseTileSelectors(onClickCallback = null) {

    $('.tile-selector').each(function () {
        var selector = this;
        var input = $(this).find('input');
        if ($(input).val() !== '') {
            $(selector).find(`.tile-selector__tile[data-value="${$(input).val()}"]`).addClass('selected');
        }

        $(this).on('click', '.tile-selector__tile', function () {
            $(selector).find('.tile-selector__tile').removeClass('selected');
            $(this).addClass('selected');
            input.val($(this).data('value'));

            if (onClickCallback !== null) {
                onClickCallback($(this).data('type'), $(this).data('value'));
            }
        });
    });

};

export function refreshTileSelectors() {

    $('.tile-selector').each(function () {
        var selector = this;
        var input = $(this).find('input');

        // Deselect any hidden values
        if ($(selector).find('.tile-selector__tile.selected:not(:visible)').length > 0) {
            $(selector).find('.tile-selector__tile').removeClass('selected');
            input.val('');
        }

        // Preselect value if theres only one
        if ($(selector).find('.tile-selector__tile.selected:visible').length == 1) {
            let selectedTile = $(selector).find('.tile-selector__tile.selected:visible');
            selectedTile.addClass('selected');
            input.val(selectedTile.data('value'));
        }
    });

};