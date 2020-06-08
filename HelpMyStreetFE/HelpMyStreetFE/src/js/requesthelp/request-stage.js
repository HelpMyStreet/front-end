import { validateFormData, validatePrivacyAndTerms, scrollToFirstError } from "../shared/validator";
import { buttonLoad, buttonUnload } from "../shared/btn";
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
        buttonLoad($("#btnNext"));
        const valid = validateFormData($(this), {
            "currentStep.SelectedTask.Id": (v) => v !== "" || "Please select at least one task type",   
            "currentStep.SelectedRequestor.Id": (v) => v !== "" || "Please select from one of the available options",                    
            "currentStep.SelectedTimeFrame.Id": (v) => {                
                if ($("#WhenNeeded").is(":visible")) {
                    if (v == "") {                     
                        return "Please tell us when you need this to be done by";
                    }
                }
                return true;
            },   
            "currentStep.AgreeToTerms": (v) =>  validatePrivacyAndTerms("currentStep.AgreeToPrivacy", "currentStep.AgreeToTerms") || "",                        
        });

        const validForm = validateQuestions() && valid;        

        if (validForm == false) {
            buttonUnload($("#btnNext"));;
            scrollToFirstError();
        }

        
        return validForm;
    });
}


var validateQuestions = function(){
    var validQuestions = [];
    $('.question').each(function () {
        var type = $(this).attr("type");
        let errorField = $(this).find("~ .error");
        if (type == "radio") {
            errorField = $(this).parentsUntil(".input").parent().find(".error");            
        }
        errorField.hide();
        var isRequired = $(this).attr("data-required");
    
        var val = $(this).val();
        if (type == "radio") {
            val = $(`input[name="${$(this).attr("name")}"]:checked`).val();
        }                
        if (isRequired == "True") {            
            if (val == undefined || val == "") {
                validQuestions.push(false);           
                errorField.text($(this).attr("data-val-message")).show();
            } else {
                validQuestions.push(true);
            }
        } else {
            validQuestions.push(true);
        }        
    });

    return !validQuestions.includes(false);
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

    $('.questions').each(function () {
        qRequest.position = $(this).attr("data-position");
        var el = $(this);
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
                el.html(data);
                if (taskId == 2) {
                    $('#requestorFor_3').show();
                    if ($('#requestorFor_3').hasClass("selected")) {
                        $('input[name="currentStep.SelectedRequestor.Id"]').val($('#requestorFor_3').attr("data-id"));
                    }
                    displayTodayHelpNeededOptions(false);
                } else {
                    $('#requestorFor_3').hide();
                    var currentRequestedFor = $('input[name="currentStep.SelectedRequestor.Id"]').val();                    
                    if (currentRequestedFor == $('#requestorFor_3').attr("data-id")) {
                        $('input[name="currentStep.SelectedRequestor.Id"]').val("");
                    }                    
                    displayTodayHelpNeededOptions(true)
                }
            }
        });
    })


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