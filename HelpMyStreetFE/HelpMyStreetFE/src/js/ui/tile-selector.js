export function initialiseTileSelector() {

  $('.tile-selector').each(function () {
    var selector = this;
    var input = $(this).find('input');
    if ($(input).val() !== '') {
      $(selector).find(`.tile-selector__tile[data-value="${$(input).val()}"]`).addClass('selected');
    }

    $(this).on('click', '.tile-selector__tile', function () {
      $(selector).find('.tile-selector__tile').removeClass('selected');
      input.val($(this).data('value'));
      $(this).addClass('selected');
    });
  });

};