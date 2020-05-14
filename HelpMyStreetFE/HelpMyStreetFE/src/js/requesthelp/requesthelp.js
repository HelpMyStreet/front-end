import { intialiseRequestStage, requestStage } from "./request-stage";
import { initaliseDetailStage, detailStage } from "./detail-stage";
import { intialiseReviewStage, reviewStage } from "./review-stage";

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
            $('.btnSubmit').show();
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
        $('.btnSubmit').hide();
        $('.btnNext').show();
    }
    $('.btnNext').click(function () {
        var activeTab = $('.progress-bar').find('.is-active');
        var currentTab = activeTab.attr("data-tab");
        var nextTab = activeTab.next().attr("data-tab");
        validateTab(currentTab).then(function (valid) {
            if (valid == true) {
                _moveTab(currentTab, nextTab)
                changeProgressNext($('.btnNext'));
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
            } else {
                var requestHelp = new Object();
                requestHelp.request = requestStage;
                requestHelp.detail = detailStage;
                intialiseReviewStage(requestHelp);
                intialiseSubmit();
            }      
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


var intialiseSubmit = function () {
    $('.btnSubmit').click(function () {        
        var requestor = {
            firstname: detailStage.yourDetails.firstname.val,
            lastname: detailStage.yourDetails.lastname.val,
            email: detailStage.yourDetails.email.val,
            mobile: detailStage.yourDetails.mobilenumber.val,
            altNumber: detailStage.yourDetails.altnumber.val,
            address: {
                addressline1: detailStage.yourDetails.address.addressLine1.val,
                addressline2: detailStage.yourDetails.address.addressLine2.val,
                locality: detailStage.yourDetails.address.locality.val,
                postcode: detailStage.yourDetails.address.postcode.val
            }
        }
        var recipient = requestor;
        if (detailStage.onBehalf) {
            recipient = {
                firstname: detailStage.theirDetails.firstname.val,
                lastname: detailStage.theirDetails.lastname.val,
                email: detailStage.theirDetails.email.val,
                mobile: detailStage.theirDetails.mobilenumber.val,
                altNumber: detailStage.theirDetails.altnumber.val,
                address: {
                    addressline1: detailStage.theirDetails.address.addressLine1.val,
                    addressline2: detailStage.theirDetails.address.addressLine2.val,
                    locality: detailStage.theirDetails.address.locality.val,
                    postcode: detailStage.theirDetails.address.postcode.val,
                }
            }
        }

        var helpRequest = {
            forRequestor: !detailStage.onBehalf,
            readPrivacyNotice: requestStage.agreeToTerms.privacy,
            acceptedTerms: requestStage.agreeToTerms.terms,
            requestor: requestor,
            recipient: recipient,
            consentForContact: detailStage.consentForContact,
            specialCommunicationNeeds: reviewStage.communicationNeeds.val,
            otherDetails: reviewStage.helperAdditionalDetails.val,
        }
        var jobRequest = {
            supportActivity: requestStage.selectedActivity.val,
            details: requestStage.additonalHelpDetail.val,
            dueDays: requestStage.selectedTime.val,
            healthCritical: requestStage.selectedHealthWellBeing.val
        }

        var data = {
            helpRequest: helpRequest,
            jobRequest: jobRequest
        };
        

        fetch(`api/requesthelp/RequestHelp`, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        }).then(function () {
            console.log(test);
        })                                
    })
   

}


$.fn.isInViewport = function () {
    var elementTop = $(this).offset().top;
    var elementBottom = elementTop + $(this).outerHeight();

    var viewportTop = $(window).scrollTop();
    var viewportBottom = viewportTop + $(window).height();

    return elementBottom > viewportTop && elementTop < viewportBottom;
};