export function initialiseGenericExpanders() {
  $('body').on('click', '.generic-expander .generic-expander__controls', function (e) {
    e.preventDefault();
    $(this).toggleClass('open');
    const target = $(document).find('#' + $(this).data('expander-target'));
    target.slideToggle();
  });
}