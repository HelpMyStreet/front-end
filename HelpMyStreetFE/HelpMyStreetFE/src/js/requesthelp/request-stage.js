import { validateFormData, validatePrivacyAndTerms } from "../shared/validator";
export function intialiseRequestStage() {
    intialiseRequestTiles();
    validateForm();

    var taskId = $('input[name="currentStep.SelectedTask.Id"]').val();
    $("#CustomTime").find("select").change(function () {
        $('input[name="currentStep.SelectedTimeFrame.CustomDays"]').val($(this).val());
    });
    if (taskId != "") {
        LoadQuestions(taskId);
    }   
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

    } else {
        $("#CustomTime").hide();
    }
    el.addClass("selected");   
    $('input[name="currentStep.SelectedTimeFrame.Id"]').val(el.attr("data-id"));
}

var handleActivity = function (el) {
    $('*[data-type="activities"]').removeClass("selected");
    el.addClass("selected");
    let taskId = el.attr("data-id");
    $('input[name="currentStep.SelectedTask.Id"]').val(taskId);
    LoadQuestions(taskId);
}



var LoadQuestions = function (taskId){
    var qRequest = {
        taskId: Number(taskId),
        step: JSON.parse($('input[name="RequestStep"]').val())
    };

    $.ajax({
        url: "/RequestHelp/Questions",
        type: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        dataType: "html",
        data: JSON.stringify(qRequest),
        success: function (data) {
            $('#questions').html(data);
        }
    });
    }
