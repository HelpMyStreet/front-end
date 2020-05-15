import { intialiseRequestStage, requestStage } from "./request-stage";
import { initaliseDetailStage, detailStage } from "./detail-stage";
import { intialiseReviewStage, reviewStage } from "./review-stage";
import { buttonLoad, buttonUnload } from "../shared/btn";

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
            mobile: detailStage.yourDetails.mobilenumber.val ? detailStage.yourDetails.mobilenumber.val : '',
            altNumber: detailStage.yourDetails.altnumber.val ? detailStage.yourDetails.altnumber.val : '',
            address: {
                addressline1: detailStage.yourDetails.address.addressLine1.val,
                addressline2: detailStage.yourDetails.address.addressLine2.val ?  detailStage.yourDetails.address.addressLine2.val : '',
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
                mobile: detailStage.theirDetails.mobilenumber.val ,
                altNumber: detailStage.theirDetails.altnumber.val ,
                address: {
                    addressline1: detailStage.theirDetails.address.addressLine1.val,
                    addressline2: detailStage.theirDetails.address.addressLine2.val, 
                    locality: detailStage.theirDetails.address.locality.val,
                    postcode: detailStage.theirDetails.address.postcode.val,
                }
            }
        }

        var helpRequest = {
            ForRequestor: !detailStage.onBehalf,
            ReadPrivacyNotice: requestStage.agreeToTerms.privacy,
            AcceptedTerms: requestStage.agreeToTerms.terms,
            Requestor: requestor,
            Recipient: recipient,
            ConsentForContact: detailStage.consentForContact,
            SpecialCommunicationNeeds: reviewStage.communicationNeeds.val ,
            OtherDetails: reviewStage.helperAdditionalDetails.val,
        }
        var jobRequest = {
            SupportActivity: requestStage.selectedActivity.val,
            Details: requestStage.additonalHelpDetail.val ,
            DueDays: parseInt(requestStage.selectedTime.val),
            HealthCritical: (requestStage.selectedHealthWellBeing.val == "true")
        }

        var data = {
            HelpRequest: helpRequest,
            JobRequest: jobRequest
        };

        $('.retryMessage').hide();
        buttonLoad($("#btnSubmit"));
        fetch('api/requesthelp', {                        
            method: 'post',
            headers: {                
                'content-type': 'application/json'
            },                        
             body: JSON.stringify(data)
        }).then(function (response) {
            response.json().then(function (data) {
                console.log(data);
                if (data.hasContent == true & data.isSuccessful == true) {
                    if (data.content.fulfillable == 4 || data.content.fulfillable == 5 || data.content.fulfillable == 6) {
                        window.location.href = "/requesthelp/success";
                    } else if (data.content.fulfillable == 1 || data.content.fulfillable == 2 || data.content.fulfillable == 3) {
                        $('.retryMessage').show();
                    }
                }
            })
        }).catch(function(){

        }).finally(function () {
            buttonUnload($("#btnSubmit"));
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