import { initialiseYoti } from "../yoti";

export function initialiseVerification() {
   
    if ($('.verification-panel')[0]) { // if verification panel is being rendered 
        var maxStep = 3;
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
    }
}


function _SetUI(stepNumber, maxStep) {
    _setActive(stepNumber);
    _showOrHideNextButton(stepNumber, maxStep);
    _setTitle(stepNumber);

    if (stepNumber == maxStep) {
        initialiseYoti();
    }
}


function _setTitle(stepNumber) {
    var title = "My Verification";
    if (stepNumber > 0) {
        title += " - Step " + stepNumber;
    } else {
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
        case "step-three":
            return 3;
            break;
        default:
            return 0;
    }
}

function _setActive(stepNumber) {
    $(".verification-step").removeClass("active");

    switch (stepNumber) {
        case 1:
            $('#step-one').addClass("active");
            break;
        case 2:
            $('#step-two').addClass("active");
            break;
        case 3:
            $('#step-three').addClass("active");
            break;
        default:
            $('#landing').addClass("active");
    }

}