import { buttonLoad, buttonUnload } from "./btn";



export function showPopup(settings) {
    
    $('.error').hide();
    var popup = $('#popup');
    popup.find(".popup__content").centerPopup();
    popup.find(".popup__content__header").first().text(settings.header);
    popup.find(".popup__content__text").first().html(settings.htmlContent);
    popup.find("#popup-accept > .text").text(settings.actionBtnText);
    if (settings.cssClass) {
        popup.find(".popup__content").addClass(settings.cssClass);
    }
    if (settings.buttonCssClass) {
        popup.find("#popup-accept").addClass(settings.buttonCssClass)
    }
    popup.fadeIn(200);     

    $('#popup-accept').unbind().bind("click", async function (evt) {
        buttonLoad($(this));    
        $('.popup-close').off('click');      
        var result = await settings.acceptCallbackAsync();                
        buttonUnload($(this));
        if (result == true) {
            popup.fadeOut(100);     
        } else {            
            bindCloseClick(popup);
            $('.error').show().text(settings.messageOnFalse);
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