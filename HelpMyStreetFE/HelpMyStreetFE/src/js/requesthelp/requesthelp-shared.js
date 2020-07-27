export function loadQuestions (taskId) {
    var requestorId = $('input[name="currentStep.SelectedRequestor.Id"]').val();
    requestorId = requestorId == "" ? null : Number(requestorId);
    
    var qRequest = {
        taskId: Number(taskId),
        step: JSON.parse($('input[name="RequestStep"]').val()),
        requestorId: requestorId,
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



