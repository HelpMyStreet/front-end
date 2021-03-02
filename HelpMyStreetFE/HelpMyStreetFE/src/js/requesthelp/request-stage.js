import { validateFormData, scrollToFirstError } from "../shared/validator";
import { buttonLoad, buttonUnload } from "../shared/btn";
import { trackEvent } from "../shared/tracking-helper";
import { loadQuestions, validateQuestions } from "./requesthelp-shared.js";
import { dateValidationSchemes, datepickerLoad, validateDate } from "../shared/date-picker";

export function intialiseRequestStage() {
    intialiseRequestTiles();
    validateForm();

    $("#CustomTime").find("select").change(function () {
        $('input[name="currentStep.SelectedTimeFrame.CustomDays"]').val($(this).val());
    });

    trackEvent("Request form", "View 0.request", "", 0);

    const taskId = $('input[name="currentStep.SelectedTask.Id"]').val();
    updateOptionsForActivity(taskId);
    if ($('#datepicker').length > 0) {
        datepickerLoad('datepicker', 'dateselectionError', dateValidationSchemes.FUTURE_DATES_6M);
    }
}



var validateForm = function () {
  $("form").on("submit", function (evt) {
    buttonLoad($("#btnNext"));
    const valid = validateFormData($(this), {
      "currentStep.SelectedTask.Id": (v) => v !== "" || "Please select at least one task type",
      "currentStep.SelectedRequestor.Id": (v) => v !== "" || "Please select from one of the available options",
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


var intialiseRequestTiles = function () {
  $('.request-help .tiles__tile').click(function () {
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
  });
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
    $(`*[data-type="timeframe"][data-id="${el.attr("data-id")}"]`).addClass("selected");
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

  let scrollTop = $(window).scrollTop();

  updateOptionsForActivity(taskId);
  loadQuestions(taskId, function () {
    if (scrollTop == $(window).scrollTop()) {
      $('html, body').animate({
        scrollTop:
          $(el).parentsUntil("form").last().next().offset().top
      }, 1000);
    }
  });
}

var updateOptionsForActivity = function (taskId) {
    let requestHelpFormVariant = JSON.parse($('input[name="requestHelp"]').val()).RequestHelpFormVariant;
    if (taskId == 12) { // facemask   
        $('#requestorFor_3').parent().show(); // onbehalf of organisation
        if ($('#requestorFor_3').hasClass("selected")) {
            $('input[name="currentStep.SelectedRequestor.Id"]').val($('#requestorFor_3').attr("data-id"));
        }
        displayTodayHelpNeededOptions(false);
    } else {
        if (requestHelpFormVariant == 11 || requestHelpFormVariant == 14 || requestHelpFormVariant == 16) { // Age UK Kent Admin Pages
            $('#requestorFor_1').parent().hide(); // myself
            $('#requestorFor_2').parent().show(); // someone else
            $('#requestorFor_3').parent().hide(); // onbehalf of organisation
            displayTodayHelpNeededOptions(true);
        } else if (requestHelpFormVariant == 19) { // Sandbox
            $('#requestorFor_3').parent().show(); // onbehalf of organisation
            if (taskId == 22 || taskId == 24) { // Volunteer Support, Vaccine Support
                $('#requestorFor_1').parent().hide(); // myself
                $('#requestorFor_2').parent().hide(); // someone else
                $('#time_1').parent().hide(); // Today
                $('#time_2').parent().hide(); // 24H
                $('#time_3').parent().hide(); // Week
                $('#time_4').parent().hide(); // When convenient
                $('#time_5').parent().hide(); // Other
                $('#time_7').parent().show(); // Date and Time
                $('#start_time_7').parent().show(); // Date and Time
                $('#end_time_7').parent().show(); // Date and Time
            } else {
                $('#requestorFor_1').parent().show(); // myself
                $('#requestorFor_2').parent().show(); // someone else
                $('#time_1').parent().show(); // Today
                $('#time_2').parent().show(); // 24H
                $('#time_3').parent().show(); // Week
                $('#time_4').parent().show(); // When convenient
                $('#time_5').parent().show(); // Other
                $('#time_7').parent().hide(); // Date and Time
                $('#start_time_7').parent().hide(); // Date and Time
                $('#end_time_7').parent().hide(); // Date and Time
            }
            $('div[data-type="request-for"]').removeClass("selected");
            $('div[data-type="timeframe"]').removeClass("selected");
            $('input[name="currentStep.SelectedRequestor.Id"]').val("");
            $('input[name="currentStep.selectedTimeFrame.Id"]').val("");
        } else {
            $('#requestorFor_3').parent().hide();
            var currentRequestedFor = $('input[name="currentStep.SelectedRequestor.Id"]').val();
            // if they have previously selected On behalf of organisation,
            //set selected RequestorID as empty since that option is only available to facemasks
            if (currentRequestedFor == $('#requestorFor_3').attr("data-id")) {
                $('input[name="currentStep.SelectedRequestor.Id"]').val("");
            }
            displayTodayHelpNeededOptions(true);
        }
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

function validateDateIfNecessary() {
    if ($('input[name="currentStep.SelectedTimeFrame.Id"]').val() == "6") {
        return validateDate($('#datepicker').val(), 'datepicker', 'dateselectionError', dateValidationSchemes.FUTURE_DATES_6M);
    } else {
        return true;
    }
}