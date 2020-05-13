import { intialiseRequestStage, requestStage } from "./request-stage";
import { initaliseDetailStage, detailStage } from "./detail-stage";

$(() => {
    initaliseProgressButtons();
    intialiseRequestStage();
});


var initaliseProgressButtons = function () {
    var changeProgressNext = function (btn) {
        var activeTab = $('.progress-bar').find('.is-active');
        activeTab.removeClass("is-active");
        activeTab.addClass("is-complete");
        var nextTab = activeTab.next();
        nextTab.addClass("is-active");
        if (nextTab.next().length == 0) {
            btn.hide();
        }
        $('.btnBack').show();
    }
    var changeProgressPrev = function (btn) {
        var activeTab = $('.progress-bar').find('.is-active');
        activeTab.removeClass("is-active");
        var prevTab = activeTab.prev();
        prevTab.removeClass("is-complete");
        prevTab.addClass("is-active");
        if (prevTab.prev().length == 0) {
            btn.hide();
        }
        $('.btnNext').show();
    }
    $('.btnNext').click(function () {
        var activeTab = $('.progress-bar').find('.is-active');
        var currentTab = activeTab.attr("data-tab");
        var nextTab = activeTab.next().attr("data-tab");
        validateTab(currentTab).then(function (valid) {
            if (valid == true) {
                _moveTab(currentTab, nextTab)
                changeProgressNext($(this));
            }
        });
    });
    $('.btnBack').click(function () {
        var activeTab = $('.progress-bar').find('.is-active');
        var currentTab = activeTab.attr("data-tab");
        var previousTab = activeTab.prev().attr("data-tab");
        _moveTab(currentTab, previousTab);
        changeProgressPrev($(this));
    });   
}
async function validateTab(currentTab){
    $('#hasErrors').hide();
    var valid = true;

    switch (currentTab) {
        case "request":
            if (requestStage.validate() == false) {
                valid = false;
            } else {
                initaliseDetailStage(requestStage.selectedFor)
            }
            break;
        case "details":
            if (await detailStage.validate(requestStage.selectedFor) == false) {
                valid = false;
            }
            break;
    }

    if (!valid) {
        $('#hasErrors').show();        
    }
    return valid;
}

function _moveTab(currentTab, nextTab) {    
    $('*[data-tab-page="' + currentTab +'"]').hide();
    $('[data-tab-page="' + nextTab + '"]').show();

    if ($('.progress-bar').isInViewport() == false) {
        $([document.documentElement, document.body]).animate({
            scrollTop: ($(".progress-bar").offset().top - 50)
        }, 500);

    }
}



$.fn.isInViewport = function () {
    var elementTop = $(this).offset().top;
    var elementBottom = elementTop + $(this).outerHeight();

    var viewportTop = $(window).scrollTop();
    var viewportBottom = viewportTop + $(window).height();

    return elementBottom > viewportTop && elementTop < viewportBottom;
};