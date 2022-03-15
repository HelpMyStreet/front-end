export function loadQuestions(supportActivity, successCallback) {
    $('button#btnNext').addClass('disabled prevent-submit');

    var qRequest = {
        supportActivity: supportActivity,
        formVariant: $('input#FormVariant').val(),
        formStage: $('input#currentStep_FormStage').val(),
        groupId: parseInt($('input#ReferringGroupId').val()),
        answers: GetCurrentQuestionAnswers(),
    };

    $('.questions').attr('data-loading', true);
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
                el.removeAttr('data-loading');
                if ($('.questions[data-loading]').length == 0) {
                    $('button#btnNext').removeClass('disabled prevent-submit');
                    successCallback();
                }
            }
        });
    });
}

export function validateQuestions () {
    var validQuestions = [];
    $('.question').each(function () {
        var type = $(this).attr("type");
        let errorField = $(this).find("~ .error");
        if (type == "radio") {
            errorField = $(this).closest(".input").find(".error");
        }
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
                errorField.hide();
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



