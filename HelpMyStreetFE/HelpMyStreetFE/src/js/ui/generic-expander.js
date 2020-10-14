export function initialiseGenericExpanders() {
  $('body').on('click', 'a.generic-expander__control', function (e) {
    e.preventDefault();
    const expander = $(this).closest('.generic-expander');
    expander.toggleClass('open');
    expander.find('.generic-expander__content').slideToggle();
  });
}