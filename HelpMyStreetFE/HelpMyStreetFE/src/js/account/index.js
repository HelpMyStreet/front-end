import login from "./login";
import notification from "./notification";

export default { login, notification };

$(document).ready(function () {
    initialiseAccountNavExpanders();
    initialiseNavBadges();
});

function initialiseAccountNavExpanders() {
    $('.account__body .account__nav .account__nav__item.submenu__container').each(function () {
        const a = $(this).children('a');
        $(a).on('click', function (event) {
            subMenuToggle($(this).parent());
            event.preventDefault();
        });
        if ($(a).hasClass('selected')) {
            subMenuToggle(this, 0);
        }
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

function subMenuToggle(container, slideDuration = 400) {
    const ul = $(container).children('ul');
    if ($(ul).is(':visible')) {
        $(ul).find('.account__nav__badge').hide();
        $(ul).slideUp(slideDuration, function () {
            $(container).children('a').children('.account__nav__badge').show();
        });
    } else {
        $(container).children('a').children('.account__nav__badge').hide();
        $(ul).find('.account__nav__badge').hide();
        $(ul).slideDown(slideDuration, function () {
            $(ul).find('.account__nav__badge').show();
        });
    }
}