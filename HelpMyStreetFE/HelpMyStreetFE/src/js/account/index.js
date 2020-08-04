import login from "./login";
import notification from "./notification";

export default { login, notification };

$(document).ready(function () {
    initialiseAccountNameExpanders();
});

function initialiseAccountNameExpanders() {
    $('.account__nav .account__nav__item.submenu__container').each(function () {
        $(this).find('a').first().on('click', function (event) {
            $(this).next('ul').slideToggle();
            event.preventDefault();
        });
    });
}