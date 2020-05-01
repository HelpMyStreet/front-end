const siteNavToggle = document.getElementById('site-nav-toggle')

siteNavToggle && siteNavToggle.addEventListener('click', function () {
    $('#profile-dropdown').removeClass("dnone");
    $('#sitenavCollapsed').toggleClass('dnone');
    $('#profile-header-account').toggleClass('collapsed')
    $('#profile-dropdown, #sitenavCollapsed').toggleClass(" page-header__account__header__dropdown--collapsed");
    $('#sitenavCollapsed > #site-nav').toggleClass('collapsed')
  $('#site-nav-toggle').toggleClass('collapsed');
});

const headerLoginToggle = document.querySelectorAll('.header-login__btn--toggle');

$(headerLoginToggle).each(function() {
  $(this).click(function() {
    $('#header-login').toggleClass('collapsed');
  })
});


$('#profile-header-account').click(function () {
    if ($(this).hasClass("collapsed")) {
        $('#profile-dropdown').toggleClass("dnone");
    }
})

