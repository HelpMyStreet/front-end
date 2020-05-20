
export function showPopup(header, htmlContent, acceptCallback) {
    var popup = $('#popup');
    popup.find(".popup__content").centerPopup();
    popup.find(".popup__content__header").first().text(header);
    
    popup.find(".popup__content__text").first().html(htmlContent);
    popup.fadeIn(200);     

    $('#popup-accept').click(function (evt) {
        evt.stopImmediatePropagation();
        acceptCallback();
        popup.fadeOut(100);    
    })
    $('.popup-close').click(function () {
        evt.stopImmediatePropagation();
        popup.fadeOut(100);    
    })
}


jQuery.fn.centerPopup = function () {
    this.css("top", Math.max(0, (($(window).height() - $(this).outerHeight()) / 2) +
        $(window).scrollTop()) + "px");
    this.css("left", Math.max(0, (($(window).width() - $(this).outerWidth()) / 2) +
        $(window).scrollLeft()) + "px");
    return this;
}