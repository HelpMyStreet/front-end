const siteNavToggle = document.getElementById('site-nav-toggle')

siteNavToggle && siteNavToggle.addEventListener('click', function() {
  $('#site-nav').toggleClass('collapsed');
  $('#site-nav-toggle').toggleClass('collapsed');
});

const headerLoginToggle = document.querySelectorAll('.header-login__btn--toggle');

$(headerLoginToggle).each(function() {
  $(this).click(function() {
    $('#header-login').toggleClass('collapsed');
  })
});
