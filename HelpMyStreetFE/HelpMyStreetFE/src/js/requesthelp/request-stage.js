import { validateFormData, validatePrivacyAndTerms } from "../shared/validator";
export function intialiseRequestStage() {
    intialiseRequestTiles();
    validateForm();
}



var validateForm = function () {
    $("form").on("submit", function (evt) {        
        const valid = validateFormData($(this), {
            "currentStep.SelectedTask.Id": (v) => v !== "" || "Please select a task",   
            "currentStep.SelectedRequestor.Id": (v) => v !== "" || "Please Health Critical or not", 
            "currentStep.SelectedTimeFrame.Id": (v) => v !== "" || "Please select Timeframe",
            "currentStep.IsHealthCritical": (v) => v !== undefined || "Please select if its health critical or not", 
            "currentStep.AgreeToTerms": (v) =>  validatePrivacyAndTerms("currentStep.AgreeToPrivacy", "currentStep.AgreeToTerms") || "",                        
        });

        return valid;

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




