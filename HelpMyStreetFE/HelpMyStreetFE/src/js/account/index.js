import login from "./login";
import notification from "./notification";

export default { login, notification };

$(document).ready(function () {
    initialiseAccountNavExpanders();
    initialiseNavBadges();
});

function initialiseAccountNavExpanders() {
    $('.account__body .account__nav .account__nav__item.submenu__container').each(function () {
        $(this).find('a').first().on('click', function (event) {
            $(this).next('ul').slideToggle();
            event.preventDefault();
        });
    });
}

function initialiseNavBadges() {
    $('.account__nav .account__nav__badge').each(function () {
        const badge = $(this);
        fetch('/account/NavigationBadge?groupKey=' + $(this).data('group-key') + '&menuPage=' + $(this).data('menu-page'))
            .then(function (response) {
                response.json().then(function (json) {
                    if (json.count > 0) {
                        $(badge).html(json.count);
                        $(badge).addClass('count');
                    } else {
                        $(badge).html('');
                        $(badge).removeClass('count');
                    }
                });
            });
    });
}