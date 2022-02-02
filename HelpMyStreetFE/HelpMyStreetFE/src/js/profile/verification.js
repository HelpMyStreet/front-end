var uaParser = require('ua-parser-js');
import { initialiseYoti } from "./yoti";
import { updateQueryStringParam, getParameterByName } from "../shared/querystring-helper";

export function initialiseVerification(isVerified) {

    if (!isVerified) {
        updateQueryStringParam("u", initObj.userId);
    }
    if ($("#verification-panel").is(":visible")) {
        var maxStep = 2;

        $('#reload-yoti').click(function () {
            $(".yoti__auth__button").hide(); 
            // reload yoti button but wait a second before we do incase we need to wait for the websockets to finish whatever they are doing 
            setTimeout(function () {
                initialiseYoti();
                $(".yoti__auth__button").show();
            }, 1000)
            
            
        })
        $('.btnNext').click(function () {
            var activeStep = $('.verification-step.active')[0].id;
            var stepNumber = _getStepNumber(activeStep) + 1;
            _SetUI(stepNumber, maxStep);
        })

        $('.btnPrev').click(function () {
            var activeStep = $('.verification-step.active')[0].id;
            var stepNumber = _getStepNumber(activeStep) - 1;
            _SetUI(stepNumber, maxStep);
        })

        $('.btnBackToStart').click(function () {
            _SetUI(0, maxStep);
        })

        var auth = getParameterByName("auth");
        if (auth == "failed") {
            _SetUI(3, maxStep);
            $('.auth-error').show();
        }
        // Listen for the event.
        document.getElementById("verification-panel").addEventListener('failed-auth', function (e) {
            _SetUI(3, maxStep);
            $('.auth-error').show();
        }, false);
    }
}


function _SetUI(stepNumber, maxStep) {
    $('.auth-error').hide();
    _setActive(stepNumber);
    _showOrHideNextButton(stepNumber, maxStep);
    _setTitle(stepNumber);

    if (stepNumber == maxStep) {
        initialiseYoti();
    }
    
    if ($('#verification-title').isInViewport() == false) {
        $([document.documentElement, document.body]).animate({
            scrollTop: ($("#verification-title").offset().top - 70)
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
function _setTitle(stepNumber) {
    var title = "My Verification";
    if (stepNumber == 0) {
         title = "My Activity";
    }
    $('#verification-title').text(title);
}

function _showOrHideNextButton(stepNumber, maxStep){
    if (stepNumber == maxStep) {
        $('.btnNext').hide();
    } else {
        $('.btnNext').show();
    }
}


function _getStepNumber(stepId) {
    
    switch (stepId) {
        case "step-one":
            return 1;
            break;
        case "step-two":
            return 2;
            break;
        default:
            return 0;
    }
}

function _setActive(stepNumber) {
    $(".verification-step").removeClass("active");

    //I want to use optional chaining here but noooooooo, IE won't let me.

    var parser = uaParser();
    var isMobile = false;
    if (parser.device) {
        isMobile = (parser.device.type == "mobile");
    }

    switch (stepNumber) {
        case 1:
            $('#step-one').addClass("active");
            break;
        case 2:
            $('#step-two').addClass("active");
            if (isMobile) {
                $('.verification-step__yoti__auth__desktop').css("display", "none");
                $('#reload-yoti').hide();
            } else {
                $('.verification-step__yoti__auth__mobile').css("display", "none");
            }
            break;
        default:
            $('#landing').addClass("active");
    }

}



