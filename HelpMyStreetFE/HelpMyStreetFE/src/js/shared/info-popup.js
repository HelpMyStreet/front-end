$('.info-popup').each(function() {
  $(this).get(0).addEventListener('click', toggleState);
});

function toggleState(e) {
  $('.info-popup__body', e.currentTarget).toggleClass('open');
}
