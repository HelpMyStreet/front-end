import { buttonLoad, buttonUnload } from "./btn";
import { hmsFetch, hmsSubmit, fetchResponses } from "./hmsFetch"

export async function showServerSidePopup(source, settings = {}, form = null) {
  var popup = $('#popup-template').clone().attr("id", "").prependTo('body');

  if (settings.noFade) {
    popup.show();
  } else {
    popup.fadeIn(200);
  }
  popup.find(".popup__content").centerPopup();

  var response = form ? await hmsSubmit(source, form) : await hmsFetch(source);
  if (response.fetchResponse == fetchResponses.SUCCESS) {
    popup.find(".popup__content").first().replaceWith(await response.fetchPayload);
    popup.find(".popup__content").centerPopup();
  } else {
    popup.find(".popup__content__header").first().text("That didn't work.");
    popup.find(".popup__content__text").first().html("<p>Sorry, we couldn't load this popup.  Please try again.</p>");
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
        if (settings.acceptCallbackAsync) {
            buttonLoad($(this));
            popup.find('.popup__content__buttons .error').hide();
            popup.find('.popup-close').off('click');
            var result = await settings.acceptCallbackAsync();
            buttonUnload($(this));
            if (result == true) {
                hidePopup(popup);
            } else {
                bindCloseClick(popup);
                if (typeof result == 'string') {
                    popup.find('.popup__content__buttons .error').text(result);
                } else if (settings.messageOnFalse) {
                    popup.find('.popup__content__buttons .error').text(settings.messageOnFalse);
                }
                popup.find('.popup__content__buttons .error').show();
            }
        } else {
            hidePopup(popup);
        }
    });
}

function bindRejectClick(popup, settings) {
  popup.find('#popup-reject').unbind().bind("click", async function (evt) {
    if (settings.rejectCallbackAsync) {
      popup.find('.popup__content__buttons .error').hide();
      popup.find('.popup-close').off('click');
      var result = await settings.rejectCallbackAsync();
      if (result == true) {
        hidePopup(popup);
      } else {
        bindCloseClick(popup);
        if (typeof result == 'string') {
          popup.find('.popup__content__buttons .error').text(result);
        } else if (settings.messageOnFalse) {
          popup.find('.popup__content__buttons .error').text(settings.messageOnFalse);
        }
        popup.find('.popup__content__buttons .error').show();
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
    this.css("top", Math.max($(window).scrollTop(), (($(window).height() - $(this).outerHeight()) / 2) +
        $(window).scrollTop()) + "px");
    return this;
}