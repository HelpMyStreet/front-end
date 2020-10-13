import { buttonLoad, buttonUnload } from "./btn";
import { hmsFetch, fetchResponses } from "./hmsFetch"

export async function showServerSidePopup(source, settings) {
  var popup = $('#popup-template').clone().attr("id", "").prependTo('body');
  popup.find(".popup__content").centerPopup();

  if (settings.noFade) {
    popup.show();
  } else {
    popup.fadeIn(200);
  }
  var response = await hmsFetch(source);
  if (response.fetchResponse == fetchResponses.SUCCESS) {
    popup.find(".popup__content__wrapper").first().html(await response.fetchPayload);
  } else {
    popup.find(".popup__content__header").first().text("That didn't work.");
    popup.find(".popup__content__text").first().html("<p>Sorry, we couldn't load this popup.  Please try again.</p>");
  }

  bindAcceptClick(popup, settings);
  bindRejectClick(popup, settings);
  bindCloseClick(popup);

  return popup;
}

export async function showPopup(settings) {
  var popup = $('#popup-template').clone().attr("id", "").prependTo('body');
  popup.find(".popup__content").centerPopup();

  popup.find(".popup__content__header").first().text(settings.header);
  popup.find(".popup__content__text").first().html(settings.htmlContent);

  popup.find("#popup-accept > .text").text(settings.actionBtnText);
  if (settings.cssClass) {
    popup.find(".popup__content").addClass(settings.cssClass);
  }
  popup.find(".popup__content__buttons").first().removeClass('dnone');
  if (settings.rejectBtnText) {
    popup.find('#popup-reject').parent().removeClass('dnone');
    popup.find("#popup-reject > .text").text(settings.rejectBtnText);
  }

  if (settings.noFade) {
    popup.show();
  } else {
    popup.fadeIn(200);
  }

  bindAcceptClick(popup, settings);
  bindRejectClick(popup, settings);
  bindCloseClick(popup);

  return popup;
}

export function hidePopup(popup, duration = 100) {
  popup.fadeOut(duration, () => { popup.remove(); });
}

function bindAcceptClick(popup, settings) {
  popup.find('#popup-accept').unbind().bind("click", async function (evt) {
    buttonLoad($(this));
    popup.find('.popup-close').off('click');
    var result = await settings.acceptCallbackAsync();
    buttonUnload($(this));
    if (result == true) {
      hidePopup(popup);
    } else {
      bindCloseClick(popup);
      if (settings.messageOnFalse) {
        popup.find('.error').text(settings.messageOnFalse);
      }
      popup.find('.error').show();
    }
  });
}

function bindRejectClick(popup, settings) {
  popup.find('#popup-reject').unbind().bind("click", async function (evt) {
    if (settings.rejectCallbackAsync) {
      popup.find('.popup-close').off('click');
      var result = await settings.rejectCallbackAsync();
      if (result == true) {
        hidePopup(popup);
      } else {
        bindCloseClick(popup);
        if (settings.messageOnFalse) {
          popup.find('.error').text(settings.messageOnFalse);
        }
        popup.find('.error').show();
      }
    } else {
      hidePopup(popup);
    }
  });
}

function bindCloseClick(popup) {
  popup.find('.popup-close').on('click', (function (evt) {
    evt.stopImmediatePropagation();
    popup.fadeOut(100, () => { popup.remove(); });
  }));
}

jQuery.fn.centerPopup = function () {
  this.css("top", Math.max(0, (($(window).height() - $(this).outerHeight()) / 2) +
    $(window).scrollTop()) + "px");
  this.css("left", Math.max(0, (($(window).width() - $(this).outerWidth()) / 2) +
    $(window).scrollLeft()) + "px");
  return this;
}