import login from "./login";
import notification from "./notification";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch";
import { getInactivityState, INACTIVITY_STATES } from "../shared/inactivity-monitor";
import { InitialiseReports} from "../report.js"

export default { login, notification };

$(document).ready(function () {
    initialiseAccountNavExpanders();
    initialiseNavBadges();
    InitialiseReports();

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
        refreshBadge(badge);
        setInterval(async function () {
            refreshBadge(badge);
        }, 5000);
    });
}

async function refreshBadge(badge) {
  if (await getInactivityState() == INACTIVITY_STATES.INACTIVE) {
    return;
  }

  var response = await hmsFetch('/account/get-navigation-badge?groupKey=' + $(badge).data('group-key') + '&menuPage=' + $(badge).data('menu-page'));
  if (response.fetchResponse == fetchResponses.SUCCESS) {
    var newCount = await response.fetchPayload;
    if ($(badge).find('.number').html() != newCount) {
      if (newCount > 0) {
          $(badge).removeClass('dnone');
          $(badge).addClass('updated');
      } else {
        $(badge).addClass('dnone');
      }
      $(badge).find('.number').html(newCount);
    } else {
      $(badge).removeClass('updated');
    }
  }
    
    
}



function subMenuToggle(container, slideDuration = 400) {
    const ul = $(container).children('ul');
    if ($(ul).is(':visible')) {
        $(ul).find('.account__nav__badge').hide();
        $(ul).slideUp(slideDuration, function () {
            $(container).children('a').children('.account__nav__badge:not(.dnone)').show();
        });
    } else {
        $(container).children('a').children('.account__nav__badge').hide();
        $(ul).find('.account__nav__badge').hide();
        $(ul).slideDown(slideDuration, function () {
            $(ul).find('.account__nav__badge:not(.dnone)').show();
        });
    }
}
