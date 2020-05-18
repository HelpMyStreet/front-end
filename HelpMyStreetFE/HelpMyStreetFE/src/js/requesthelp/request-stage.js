export var requestStage = {
    selectedActivity:{ val: null, errorSpan: "e-activity" },
    selectedTime: { val: null, id: null, errorSpan: "e-time-frame" },
    selectedFor: { val: null, errorSpan: "e-help-for" },
    selectedHealthWellBeing: { val: null, errorSpan: "e-critcal" },
    additonalHelpDetail: { val: null, errorSpan: "e-additional-help" },
    agreeToTerms: {
        privacy: false,
        terms: false,
    },
    validate:function () {
        $('.error').hide();
        let valid = true;
        if (!requestStage.selectedActivity.val) {
            $('#' + requestStage.selectedActivity.errorSpan).show().text("Please select at least one task type");
            valid = false;
        } else if (requestStage.selectedActivity.val == "Other" && (requestStage.additonalHelpDetail.val == "" || !requestStage.additonalHelpDetail.val)) {
            $('#' + requestStage.additonalHelpDetail.errorSpan).show().text("Please provide a brief description of the help you need");
            valid = false;
        }

        if (requestStage.additonalHelpDetail.val && requestStage.additonalHelpDetail.val.length >= 1000) {
            $('#' + requestStage.additonalHelpDetail.errorSpan).show().text("Sorry, we can only accept up to 1000 characters");
        }

        if (!requestStage.selectedTime.val) {
            $('#' + requestStage.selectedTime.errorSpan).show().text("Please tell us when you need this to be done by");
            valid = false;
        }
        if (!requestStage.selectedFor.val) {
            $('#' + requestStage.selectedFor.errorSpan).show().text("Please select from one of the available options");
            valid = false;
        }
        if (!requestStage.selectedHealthWellBeing.val) {
            $('#' + requestStage.selectedHealthWellBeing.errorSpan).show().text("Please select from one of the available options");
            valid = false;
        }
        if (!validatePrivacyAndTerms())
            valid = false;

        return valid;
    }
}

export function intialiseRequestStage() {
    intialiseRequestTiles();
    intialiseHealthWellBeingCheckbox();
    intialiseAgreeToTerms();
    intialiseAdditonalDetail();

    $('form').submit(function (e) {
        e.preventDefault();
    })
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
    })
}
var handleRequestFor = function (el) {
    $('*[data-type="request-for"]').removeClass("selected");
    el.addClass("selected");  
    requestStage.selectedFor.val = el.attr("id");    
}
var handleTimeFrame = function (el) {
    $('*[data-type="timeframe"]').removeClass("selected");
    let allowCustomEntry = el.attr("data-allowcustom");
    let selectedValue = el.attr("data-val");
    requestStage.selectedTime.val = selectedValue
    requestStage.selectedTime.id = el.attr("id");
    if (allowCustomEntry == "True") {
        $("#CustomTime").show();
        $("#CustomTime").find("select").change(function () {
            requestStage.selectedTime.val = $(this).val();                      
        });
    } else {
        $("#CustomTime").hide();
    }
    el.addClass("selected");   
}
var handleActivity = function (el) {
    $('*[data-type="activities"]').removeClass("selected");
    el.addClass("selected");
    requestStage.selectedActivity.val = el.attr("id");    
}
var intialiseHealthWellBeingCheckbox = function () {    
    $('input[name=volunteer_medical_condition]').parent().removeClass("selected");
    $('input[name=volunteer_medical_condition]').prop('checked', false);

    $('input[name="volunteer_medical_condition"]').change(function (el) {
        let selected = $('input[name=volunteer_medical_condition]:checked');
        $('input[name=volunteer_medical_condition]').parent().removeClass("selected");
        selected.parent().addClass("selected");
        requestStage.selectedHealthWellBeing.val = selected.val();       
    })
}
var intialiseAdditonalDetail = function () {
    $('textarea[name="additional-help-detail"]').val("");

    $('textarea[name="additional-help-detail"]').blur(function () {        
        requestStage.additonalHelpDetail.val = $(this).val();
    });
}
var intialiseAgreeToTerms = function () {
    $('input[name="privacy_notice"]').prop('checked', false);
    $('input[name="terms_and_conditions"]').prop('checked', false);

    $('input[name="privacy_notice"]').change(function () {
        requestStage.agreeToTerms.privacy = $(this).is(":checked");
    })

    $('input[name="terms_and_conditions"]').change(function () {
        requestStage.agreeToTerms.terms = $(this).is(":checked");
    })    
}
function validatePrivacyAndTerms() {
    // requires checking of two or more inputs at the same time, so cant use the validateFormData.
    $('#e-terms-privacy').hide();
    let privacy = $("input[name='privacy_notice']").is(":checked");
    let terms = $("input[name='terms_and_conditions']").is(":checked");
    let errorText = "";
    privacy == false && terms == false ? errorText = "Please tick to indicate that you acknowledge our Privacy Policy and accept our Terms and Conditions." : "";
    privacy == true && terms == false ? errorText = "Please tick to confirm that you agree to the Help My Street <a href='/terms-conditions'>Terms and Conditions</a>" : "";
    privacy == false && terms == true ? errorText = "Please tick to confirm that you acknowledge the Help My Street <a href='/privacy-policy'>Privacy Notice</a>" : "";

    $('#e-terms-privacy').show();
    $('#e-terms-privacy').html(errorText);

    if (errorText !== "") {
        return false;
    }
    return true;
}


