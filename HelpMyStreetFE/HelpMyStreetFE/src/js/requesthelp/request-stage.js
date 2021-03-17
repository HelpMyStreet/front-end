﻿import { validateFormData, scrollToFirstError } from "../shared/validator";
import { buttonLoad, buttonUnload } from "../shared/btn";
import { trackEvent } from "../shared/tracking-helper";
import { loadQuestions, validateQuestions } from "./requesthelp-shared.js";
import { dateValidationSchemes, datepickerLoad, validateDate } from "../shared/date-picker";
import { initialiseTileSelectors, refreshTileSelectors } from "../ui/tile-selector";

export function intialiseRequestStage() {
    initialiseTileSelectors(onTileSelected);
    validateForm();

    trackEvent("Request form", "View 0.request", "", 0);

    if ($('#datepicker').length > 0) {
        datepickerLoad('datepicker', 'dateselectionError', dateValidationSchemes.FUTURE_DATES_6M);
    }

    if ($('.activity-selector-container input').val() !== '') {
        console.log($('.activity-selector-container input').val());
        updateOptionsForActivity($('.activity-selector-container input').val());
        refreshTileSelectors();
    }
}



var validateForm = function () {
    $("form").on("submit", function (evt) {
        buttonLoad($("#btnNext"));
        const valid = validateFormData($(this), {
            "currentStep.SelectedTask": (v) => v !== "" || "Please select at least one task type",
            "currentStep.SelectedRequestor": (v) => v !== "" || "Please select from one of the available options",
            "currentStep.SelectedFrequency": (v) => v !== "" || "Please tell us how often the help is needed",
            "currentStep.SelectedTimeFrame.Id": (v) => v !== "" || "Please tell us when you need this to be done by",
            "currentStep.AgreeToPrivacyAndTerms": (v) => v === true || "Please tick to indicate that you acknowledge our Privacy Policy and accept our Terms and Conditions.",
        });

        const validForm = validateQuestions() && validateDateIfNecessary() && valid;

        trackEvent("Request form", "Submit 0.request", validForm ? "(Valid)" : "(Invalid)", 0);

        if (validForm == false) {
            buttonUnload($("#btnNext"));;
            scrollToFirstError();
        }

        return validForm;
    });
}

var onTileSelected = function (type, value, triggeredByUserAction) {
    if (type === 'activities') {
        let scrollTop = $(window).scrollTop();

        updateOptionsForActivity(value);
        refreshTileSelectors();
        if (triggeredByUserAction) {
            loadQuestions(value, function () {
                if (scrollTop == $(window).scrollTop()) {
                    $('html, body').animate({
                        scrollTop:
                            $('.activity-selector-container').next().offset().top
                    }, 1000);
                }
            });
        }
    } else if (type === 'frequency') {
        updateOptionsForFrequency(value);
    }
}

var updateOptionsForActivity = function (supportActivity) {
    if (supportActivity == 'FaceMask') {
        $('#requestorFor_1').parent().show(); // myself
        $('#requestorFor_2').parent().show(); // someone else
        $('#requestorFor_3').parent().show(); // onbehalf of organisation

        $('*[data-type="timeframe"]').each(function () { $(this).parent().hide(); });

        $('*[data-type="timeframe"][data-duedatetype="Before"]').each(function () {
            const days = parseInt($(this).data('val'));
            if (days < 7) { $(this).parent().hide(); }
            else { $(this).parent().show(); }
        });
    } else if (supportActivity == 'VolunteerSupport' || supportActivity == 'VaccineSupport') {
        $('#requestorFor_1').parent().hide(); // myself
        $('#requestorFor_2').parent().hide(); // someone else
        $('#requestorFor_3').parent().show(); // onbehalf of organisation

        $('*[data-type="timeframe"]').each(function () { $(this).parent().hide(); });
        $('*[data-type="timeframe"][data-duedatetype="SpecificStartAndEndTimes"]').each(function () { $(this).parent().show(); });
    } else {
        $('#requestorFor_1').parent().show(); // myself
        $('#requestorFor_2').parent().show(); // someone else
        $('#requestorFor_3').parent().hide(); // onbehalf of organisation

        $('*[data-type="timeframe"]').each(function () { $(this).parent().show(); });
        $('*[data-type="timeframe"][data-duedatetype="SpecificStartAndEndTimes"]').each(function () { $(this).parent().hide(); });
    }
}

function validateDateIfNecessary() {
    if ($('input[name="currentStep.SelectedTimeFrame.Id"]').val() == "6") {
        return validateDate($('#datepicker').val(), 'datepicker', 'dateselectionError', dateValidationSchemes.FUTURE_DATES_6M);
    } else if ($('input[name="currentStep.SelectedTimeFrame.Id"]').val() == "7") {
        let valid = true;
        if (!validateDate($('#datepicker').val(), 'datepicker', 'dateselectionError', dateValidationSchemes.FUTURE_DATES_6M)) {
            valid = false;
        }
        if ($('input[name="currentStep.SelectedTimeFrame.StartTime"]').val() == "") {
            $('#starttimeselectionError').show().text("Please enter a start time");
            $('input[name="currentStep.SelectedTimeFrame.StartTime"]').on('blur', function () { $('#starttimeselectionError').hide(); });
            valid = false;
        }
        if ($('input[name="currentStep.SelectedTimeFrame.EndTime"]').val() == "") {
            $('#endtimeselectionError').show().text("Please enter an end time");
            $('input[name="currentStep.SelectedTimeFrame.EndTime"]').on('blur', function () { $('#endtimeselectionError').hide(); });
          valid = false;
        }
        return valid;
    } else {
        return true;
    }
}