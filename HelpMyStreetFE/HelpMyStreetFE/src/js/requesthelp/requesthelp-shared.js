export function loadQuestions(supportActivity) {
    var requestorId = $('input[name="currentStep.SelectedRequestor.Id"]').val();
    requestorId = requestorId == "" ? null : Number(requestorId);
    
    var qRequest = {
        SupportActivity: supportActivity,
        formVariant: $('input#FormVariant').val(),
        formStage: $('input#currentStep_FormStage').val(),
        requestorId: requestorId,
        previousAnswers: JSON.parse($('input#currentStep_PreviousAnswers').val()),
        answers: GetCurrentQuestionAnswers(),
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
            }
        });
    })
}

export function validateQuestions () {
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




var GetCurrentQuestionAnswers = function () {
    var questionAnswers = []

    $('.question').each(function () {
        var type = $(this).attr("type");
        var val = $(this).val();
        if (type == "radio") {
            val = $(`input[name="${$(this).attr("name")}"]:checked`).val();
        }
        if (val != undefined) {
            questionAnswers.push({
                id: Number($(this).attr("data-id")),
                answer: val
            });
        }
    });

    return questionAnswers;
}



