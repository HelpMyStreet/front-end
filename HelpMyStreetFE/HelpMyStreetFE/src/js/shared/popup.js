import { buttonLoad, buttonUnload } from "./btn";
export function showPopup(header, htmlContent, messageOnFalse, acceptCallbackAsync) {
    $('.error').hide();
    var popup = $('#popup');
    popup.find(".popup__content").centerPopup();
    popup.find(".popup__content__header").first().text(header);
    
    popup.find(".popup__content__text").first().html(htmlContent);
    popup.fadeIn(200);     

    $('#popup-accept').unbind().bind("click", async function (evt) {
        buttonLoad($(this));
        $('.popup-close').off('click');
        var result = await acceptCallbackAsync();
        buttonUnload($(this));
        if (result == true) {
            popup.fadeOut(100);     
        } else {            
            bindCloseClick(popup);
            $('.error').show().text(messageOnFalse);
        }
        
    })

    bindCloseClick(popup);
}


function bindCloseClick(popup) {
    $('.popup-close').on('click', (function (evt) {
        evt.stopImmediatePropagation();
        popup.fadeOut(100);
    }));
}

jQuery.fn.centerPopup = function () {
    this.css("top", Math.max(0, (($(window).height() - $(this).outerHeight()) / 2) +
        $(window).scrollTop()) + "px");
    this.css("left", Math.max(0, (($(window).width() - $(this).outerWidth()) / 2) +
        $(window).scrollLeft()) + "px");
    return this;
}