$(function () {
    const stickyNav = document.getElementById("sticky-nav");
    const firstSignUpButton = $('.container .btn--sign-up').first();

    if (stickyNav !== null) {
        window.onscroll = function () {
            let firstSignUpButtonTop = firstSignUpButton.offset().top;
            if (window.innerWidth < 1024) {
                if (window.pageYOffset > firstSignUpButtonTop) {
                    $("#sticky-nav").fadeIn(500).css({ display: 'flex' });

                } else {
                    $("#sticky-nav").fadeOut(500);
                }
            } else {
                $("#sticky-nav").fadeOut(500);
            }
        }
    }
});