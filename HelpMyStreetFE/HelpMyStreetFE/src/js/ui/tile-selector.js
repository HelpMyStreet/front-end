export function initialiseTileSelectors(onClickCallback = null) {

    $('.tile-selector').each(function () {
        var selector = this;
        var input = $(this).find('input.tile-selector__value');
        if ($(input).val() !== '') {
            const tileToSelect = $(selector).find(`.tile-selector__tile[data-value="${$(input).val()}"]`);
            tileToSelect.addClass('selected');
            if (onClickCallback !== null) {
                onClickCallback($(tileToSelect).data('type'), $(tileToSelect).data('value'), false);
            }
        }

        $(this).on('click', '.tile-selector__tile', function () {
            $(selector).find('.tile-selector__tile').removeClass('selected');
            $(this).addClass('selected');
            input.val($(this).data('value'));

            if (onClickCallback !== null) {
                onClickCallback($(this).data('type'), $(this).data('value'), true);
            }
        });
    });

};

export function refreshTileSelectors(onClickCallback = null) {

    $('.tile-selector').each(function () {
        var selector = this;
        const visibleTiles = $(selector).find('.tile-selector__tile:visible');
        const input = $(selector).find('input.tile-selector__value');
        const value = input.val();

        // Deselect any hidden values
        if ($(selector).find('.tile-selector__tile.selected:not(:visible)').length > 0) {
            $(selector).find('.tile-selector__tile').removeClass('selected');
            input.val('');
        }

        // Preselect value if theres only one
        if (visibleTiles.length == 1) {
            visibleTiles.addClass('selected');
            input.val(visibleTiles.data('value'));
        }

        if (value !== input.val() && onClickCallback !== null) {
            onClickCallback(visibleTiles.data('type'), input.val(), false);
        }
    });

};