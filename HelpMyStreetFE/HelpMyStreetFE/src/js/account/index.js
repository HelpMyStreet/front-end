import login from "./login";
import notification from "./notification";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch";

export default { login, notification };

$(document).ready(function () {
    initialiseAccountNavExpanders();
    initialiseNavBadges();
    initialiseAwardsView();
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

function initialiseAwardsView() {
    updateAwards();
    const awardsInterval = setInterval(async function () { updateAwards() }, 15000);
}

function initialiseNavBadges() {
    $('.account__nav .account__nav__badge').each(function () {
        const badge = $(this);
        refreshBadge(badge);
        const interval = setInterval(async function () {
            refreshBadge(badge, interval);
        }, 5000);
    });
}

async function refreshBadge(badge, interval) {
  var response = await hmsFetch('/account/NavigationBadge?groupKey=' + $(badge).data('group-key') + '&menuPage=' + $(badge).data('menu-page'));
  if (response.fetchResponse == fetchResponses.SUCCESS) {
    var newCount = await response.fetchPayload;
    if ($(badge).find('.number').html() != newCount) {
      if ($(badge).is(':visible')) {
        $(badge).addClass('updated');
      } else {
        $(badge).removeClass('dnone');
      }
      $(badge).find('.number').html(newCount);
    } else {
      $(badge).removeClass('updated');
    }
  } else if (response.fetchResponse == fetchResponses.UNAUTHORISED) {
    if (window.location.pathname.startsWith('/account/')) {
      // Session expired on logged-in page; redirect to login
      window.location.replace('/account/Login?ReturnUrl=' + encodeURIComponent((window.location.pathname + window.location.search)));
    } else {
      // Session expired on public page; don't redirect, but also don't bother trying to get any more badge refreshes
      clearInterval(interval);
    }
  } else {
    // No badges today
  }

    
}

async function updateAwards() {
    var response = await hmsFetch('/account/LoadAwardsComponent');
    if (response.fetchResponse == fetchResponses.SUCCESS) {
        var html = await response.fetchPayload;
        $(".awards-component").html(html);

    } else if (response.fetchResponse == fetchResponses.UNAUTHORISED) {
        if (window.location.pathname.startsWith('/account/')) {
            // Session expired on logged-in page; redirect to login
            window.location.replace('/account/Login?ReturnUrl=' + encodeURIComponent((window.location.pathname + window.location.search)));
        } else {
            // Session expired on public page; don't redirect, but also don't bother trying to get any more badge refreshes
            clearInterval(awardsInterval);
        }
    } else {
        //something terrible has gone wrong!
    }
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
