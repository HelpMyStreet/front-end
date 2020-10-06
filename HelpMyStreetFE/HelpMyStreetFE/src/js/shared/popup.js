import { buttonLoad, buttonUnload } from "./btn";
import { hmsFetch, fetchResponses } from "./hmsFetch"

export async function showPopup(settings) {

  var popup = $('#popup-template').clone().attr("id", "").prependTo('body');
  popup.find(".popup__content").centerPopup();
  popup.find(".popup__content__header").first().text(settings.header);

  popup.find(".popup__content__text").first().html(settings.htmlContent);
  if (settings.htmlContent_source) {
    var response = await hmsFetch(settings.htmlContent_source);
    if (response.fetchResponse == fetchResponses.SUCCESS) {
      popup.find(".popup__content__text").first().html(await response.fetchPayload);
    } else {
      popup.find(".popup__content__text").first().html("Sorry, we couldn't load this content.");
    }
  }

  popup.find("#popup-accept > .text").text(settings.actionBtnText);
  if (settings.cssClass) {
    popup.find(".popup__content").addClass(settings.cssClass);
  }
  if (settings.buttonCssClass) {
    popup.find("#popup-accept").addClass(settings.buttonCssClass);
  }
  if (settings.rejectBtnText) {
    popup.find('#popup-reject').parent().removeClass('dnone');
    popup.find("#popup-reject > .text").text(settings.rejectBtnText);
  }
  if (settings.rejectbuttonCssClass) {
    popup.find("#popup-reject").addClass(settings.rejectbuttonCssClass);
  }
  if (settings.noFade) {
    popup.show();
  } else {
    popup.fadeIn(200);
  }

  popup.find('#popup-accept').unbind().bind("click", async function (evt) {
    buttonLoad($(this));
    popup.find('.popup-close').off('click');
    var result = await settings.acceptCallbackAsync();
    buttonUnload($(this));
    if (result == true) {
      hidePopup(popup);
    } else {
      bindCloseClick(popup);
      popup.find('.error').show().text(settings.messageOnFalse);
    }
  });

  if (settings.rejectBtnText) {
    popup.find('#popup-reject').unbind().bind("click", async function (evt) {
      if (settings.rejectCallbackAsync) {
        popup.find('.popup-close').off('click');
        var result = await settings.rejectCallbackAsync();
        if (result == true) {
          hidePopup(popup);
        } else {
          bindCloseClick(popup);
          popup.find('.error').show().text(settings.messageOnFalse);
        }
      } else {
        hidePopup(popup);
      }
    });
  }

  bindCloseClick(popup);

  return popup;
}

export function hidePopup(popup, duration = 100) {
  popup.fadeOut(duration, () => { popup.remove(); });
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