import { intialiseRequestStage, requestStage } from "./request-stage";
import { initaliseDetailStage, detailStage } from "./detail-stage";
import { intialiseReviewStage, reviewStage, onDirectToRequestClick, onDirectToDetailClick } from "./review-stage";
import { buttonLoad, buttonUnload } from "../shared/btn";




$(() => {
    initaliseProgressButtons();
    intialiseRequestStage();
});


var initaliseProgressButtons = function () {
    let changeProgressNext = function (btn) {
        let activeTab = $('.progress-bar').find('.is-active');
        activeTab.removeClass("is-active");
        activeTab.addClass("is-complete");
        let nextTab = activeTab.next();
        nextTab.addClass("is-active");       
        if (nextTab.next().length == 0) {
            btn.hide();
            $('.btnSubmit').show();
        } 
        
        $('.btnBack').show();
    }
    var changeProgressPrev = function (btn) {
        let activeTab = $('.progress-bar').find('.is-active');        
        activeTab.removeClass("is-active");
        let prevTab = activeTab.prev();
        prevTab.removeClass("is-complete");
        prevTab.addClass("is-active");
        if (prevTab.prev().length == 0) {
            btn.hide();           
        }
        $('.btnSubmit').hide();
        $('.btnNext').show();
    }
    $('.btnNext').click(function () {
        let activeTab = $('.progress-bar').find('.is-active');
        let currentTab = activeTab.attr("data-tab");
        let nextTab = activeTab.next().attr("data-tab");
        validateTab(currentTab).then(function (valid) {
            if (valid == true) {
                _moveTab(currentTab, nextTab)
                changeProgressNext($('.btnNext'));
            }
        });
    });

    $('.btnBack').click(function () {
        goBack();
    });  

    onDirectToDetailClick(function () {
        goBack();
    })

    onDirectToRequestClick(function () {
        goBack();
        goBack();
    })

    var goBack = function(){
        let activeTab = $('.progress-bar').find('.is-active');
        let currentTab = activeTab.attr("data-tab");
        let previousTab = activeTab.prev().attr("data-tab");
        _moveTab(currentTab, previousTab);
        changeProgressPrev($('.btnBack'));
    }
}








async function validateTab(currentTab){
    $('#hasErrors').hide();
    let valid = true;
    console.log(currentTab);

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
                let requestHelp = new Object();
                requestHelp.request = requestStage;
                requestHelp.detail = detailStage;
                intialiseReviewStage(requestHelp);
                intialiseSubmit();
            }
            break;
        default:
            valid = true;
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


var intialiseSubmit = function () {
    $('.btnSubmit').click(async function () {
        let requestor = {
            firstname: detailStage.yourDetails.firstname.val,
            lastname: detailStage.yourDetails.lastname.val,
            email: detailStage.yourDetails.email.val,
            mobile: detailStage.yourDetails.mobilenumber.val ? detailStage.yourDetails.mobilenumber.val : '',
            altNumber: detailStage.yourDetails.altnumber.val ? detailStage.yourDetails.altnumber.val : '',
            address: {
                addressline1: detailStage.yourDetails.address.addressLine1.val,
                addressline2: detailStage.yourDetails.address.addressLine2.val ? detailStage.yourDetails.address.addressLine2.val : '',
                locality: detailStage.yourDetails.address.locality.val,
                postcode: detailStage.yourDetails.address.postcode.val
            }
        }
        let recipient = !detailStage.onBehalf ? requestor : {
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

        let helpRequest = {
            ForRequestor: !detailStage.onBehalf,
            ReadPrivacyNotice: requestStage.agreeToTerms.privacy,
            AcceptedTerms: requestStage.agreeToTerms.terms,
            Requestor: requestor,
            Recipient: recipient,
            ConsentForContact: detailStage.consentForContact,
            SpecialCommunicationNeeds: reviewStage.communicationNeeds.val,
            OtherDetails: reviewStage.helperAdditionalDetails.val,
        }
        let jobRequest = {
            SupportActivity: requestStage.selectedActivity.val,
            Details: requestStage.additonalHelpDetail.val,
            DueDays: parseInt(requestStage.selectedTime.val),
            HealthCritical: (requestStage.selectedHealthWellBeing.val == "true")
        }

        let data = {
            HelpRequest: helpRequest,
            JobRequest: jobRequest
        };

        $('.retryError').hide();

        try {
            buttonLoad($("#btnSubmit"));
            let resp = await fetch('api/requesthelp', {
                method: 'post',
                headers: {
                    'content-type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            
            if (resp.ok) {
                let respData = await resp.json()                
                if (respData.hasContent == true & respData.isSuccessful == true) {
                    if (respData.content.fulfillable == 4 || respData.content.fulfillable == 5 || respData.content.fulfillable == 6 || respData.content.fulfillable == 2 ) {
                        window.location.href = "/requesthelp/success?fulfillable=" + respData.content.fulfillable + "&onBehalf=" + detailStage.onBehalf;
                    } else if (respData.content.fulfillable == 1 || respData.content.fulfillable == 3) {
                        throw 'The Request for help had an error, this could be due to an invalid postcodee';
                    }
                } else {
                    throw 'error occured from within the request service';
                }
            } else {
                throw 'error calling the request service'
            }
        } catch(e) {
            $('.retryError').show();
            console.error(e);
        } finally {
            buttonUnload($("#btnSubmit"));
        }

    });
}


$.fn.isInViewport = function () {
    let elementTop = $(this).offset().top;
    let elementBottom = elementTop + $(this).outerHeight();

    let viewportTop = $(window).scrollTop();
    let viewportBottom = viewportTop + $(window).height();

    return elementBottom > viewportTop && elementTop < viewportBottom;
};