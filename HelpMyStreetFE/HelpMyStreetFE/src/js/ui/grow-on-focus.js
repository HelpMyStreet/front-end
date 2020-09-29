$(function () {
  $('.grow-on-focus').on('focus', function () {
    $(this).removeClass('shrink');
  });
  $('input.grow-on-focus, textarea.grow-on-focus').on('blur', function () {
    if ($(this).val() === '') {
      $(this).addClass('shrink');
    }
  });
});