export function initialiseGenericExpanders() {
  $('body').on('click', '.generic-expander__controls a', function (e) {
    e.preventDefault();
    const expander = $(this).closest('.generic-expander');
    expander.toggleClass('open');
    expander.find('.generic-expander__content').slideToggle();
  });
}