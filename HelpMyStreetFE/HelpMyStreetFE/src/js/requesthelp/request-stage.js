import { validateFormData, validatePrivacyAndTerms, scrollToFirstError } from "../shared/validator";
import { buttonLoad, buttonUnload } from "../shared/btn";
import { trackEvent } from "../shared/tracking-helper";
import { loadQuestions, validateQuestions } from "./requesthelp-shared.js";

export function intialiseRequestStage() {
    intialiseRequestTiles();
    validateForm();

    $("#CustomTime").find("select").change(function () {
        $('input[name="currentStep.SelectedTimeFrame.CustomDays"]').val($(this).val());
    });

    trackEvent("Request form", "View 0.request", "", 0);

    const taskId = $('input[name="currentStep.SelectedTask.Id"]').val();
    updateOptionsForActivity(taskId);
}



var validateForm = function () {
    $("form").on("submit", function (evt) {        
        buttonLoad($("#btnNext"));
        const valid = validateFormData($(this), {
            "currentStep.SelectedTask.Id": (v) => v !== "" || "Please select at least one task type",   
            "currentStep.SelectedRequestor.Id": (v) => v !== "" || "Please select from one of the available options",                    
            "currentStep.SelectedTimeFrame.Id": (v) => v !== "" || "Please tell us when you need this to be done by",        
            "currentStep.AgreeToTerms": (v) =>  validatePrivacyAndTerms("currentStep.AgreeToPrivacy", "currentStep.AgreeToTerms") || "",                        
        });

        const validForm = validateQuestions() && valid;

        trackEvent("Request form", "Submit 0.request", validForm ? "(Valid)" : "(Invalid)", 0);

        if (validForm == false) {
            buttonUnload($("#btnNext"));;
            scrollToFirstError();
        }
        
        return validForm;
    });
}


var intialiseRequestTiles = function () {
    $('.tiles__tile').click(function () {
        let type = $(this).attr("data-type");
        switch (type) {
            case "activities":
                handleActivity($(this));    
                break;
            case "timeframe":
                handleTimeFrame($(this));
                break;
            case "request-for":
                handleRequestFor($(this));
                break;
      }

      $('html, body').animate({
        scrollTop:
          $(this).parentsUntil("form").last().next().offset().top
      }, 1000);
    })
}
var handleRequestFor = function (el) {
    $('*[data-type="request-for"]').removeClass("selected");
    el.addClass("selected");      
    $('input[name="currentStep.SelectedRequestor.Id"]').val(el.attr("data-Id"));


    var taskId = $('input[name="currentStep.SelectedTask.Id"]').val();
    if (taskId != "") {
        let selectedValue = $(el).find('.tiles__tile__content__header').first().html();
        trackEvent("Request form", "Select request for", selectedValue, 0);
    }
}
var handleTimeFrame = function (el) {
    $('*[data-type="timeframe"]').removeClass("selected");
    let allowCustomEntry = el.attr("data-allowcustom");
    if (allowCustomEntry == "True") {
        $("#CustomTime").show();

    } else {
        $("#CustomTime").hide();
    }
    el.addClass("selected");   
    $('input[name="currentStep.SelectedTimeFrame.Id"]').val(el.attr("data-id"));

    let selectedValue = $(el).find('.tiles__tile__content__header').first().html();
    trackEvent("Request form", "Select timeframe", selectedValue, 0);
}

var handleActivity = function (el) {
    $('*[data-type="activities"]').removeClass("selected");
    el.addClass("selected");
    let taskId = el.attr("data-id");
    $('input[name="currentStep.SelectedTask.Id"]').val(taskId);

    let selectedValue = $(el).find('.tiles__tile__content__header').first().html();
    trackEvent("Request form", "Select activity", selectedValue, 0);

    updateOptionsForActivity(taskId);
    loadQuestions(taskId);
}

var updateOptionsForActivity = function (taskId) {
    if (taskId == 12) { // facemask   
        $('#requestorFor_3').show(); // onbehalf of organisation
        if ($('#requestorFor_3').hasClass("selected")) {
            $('input[name="currentStep.SelectedRequestor.Id"]').val($('#requestorFor_3').attr("data-id"));
        }
        displayTodayHelpNeededOptions(false);
    } else {
        $('#requestorFor_3').hide();
        var currentRequestedFor = $('input[name="currentStep.SelectedRequestor.Id"]').val();
        // if they have previously selected On behalf of organisation,
        //set selected RequestorID as empty since that option is only available to facemasks
        if (currentRequestedFor == $('#requestorFor_3').attr("data-id")) {
            $('input[name="currentStep.SelectedRequestor.Id"]').val("");
        }
        displayTodayHelpNeededOptions(true)
    }

    // preselect value if theres only one
    if ($('.requestorFor:visible').length == 1) {
        $('.requestorFor:visible').addClass("selected");
        let selectedId = $('.requestorFor:visible').attr("data-id");
        $('input[name="currentStep.SelectedRequestor.Id"]').val(selectedId);
    }
}

function displayTodayHelpNeededOptions(show) {
    if (!show) {
        $('#time_1').parent().hide();
        $('#time_2').parent().hide();        
    } else {
        $('#time_1').parent().show();
        $('#time_2').parent().show();        
    }
}