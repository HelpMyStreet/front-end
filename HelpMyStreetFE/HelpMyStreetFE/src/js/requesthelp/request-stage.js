
export function intialiseRequestStage() {
    intialiseRequestTiles();
    intialiseHealthWellBeingCheckbox();

    //$('form').submit(function (e) {
    //    e.preventDefault();
    //})
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
    $('input[name="currentStep.SelectedRequestor.Id"]').val(el.attr("data-Id"));
}
var handleTimeFrame = function (el) {
    $('*[data-type="timeframe"]').removeClass("selected");
    let allowCustomEntry = el.attr("data-allowcustom");    
    if (allowCustomEntry == "True") {
        $("#CustomTime").show();
        $("#CustomTime").find("select").change(function () {              
            $('input[name="currentStep.SelectedTimeFrame.CustomDays"]').val($(this).val());
        });
    } else {
        $("#CustomTime").hide();
    }
    el.addClass("selected");   
    $('input[name="currentStep.SelectedTimeFrame.Id"]').val(el.attr("data-id"));
}

var handleActivity = function (el) {
    $('*[data-type="activities"]').removeClass("selected");
    el.addClass("selected");
    $('input[name="currentStep.SelectedTask.Id"]').val(el.attr("data-id"));

    $.ajax({
        url: "/RequestHelp/Questions?taskId=" + el.attr("data-id") ,
        type: "GET",
        dataType: "html",  
        success: function (data) {
            $('#questions').html(data);
        }
       
    });
}


var intialiseHealthWellBeingCheckbox = function () {    
    $('input[name="volunteer_medical_condition"]').change(function (el) {
        let selected = $('input[name=volunteer_medical_condition]:checked');
        $('input[name=volunteer_medical_condition]').parent().removeClass("selected");
        selected.parent().addClass("selected");    
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


