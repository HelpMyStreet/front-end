import { buttonLoad, buttonUnload } from "../shared/btn";
import { hmsSubmit, fetchResponses } from "../shared/hmsFetch";

export async function initialiseEditableQuestions() {
    $('body').on('click', '.editable-question .edit', function (e) {
        e.preventDefault();
        const questionContainer = $(this).closest('.editable-question');
        questionContainer.find('div.answer').hide();
        questionContainer.find('form').show();
    });

    $('body').on('click', '.editable-question .save-changes', async function (e) {
        e.preventDefault();
        buttonLoad($(this));

        const questionContainer = $(this).closest('.editable-question');

        const response = await hmsSubmit(`/api/request-help/update-job-question?j=${questionContainer.data('job-id')}&q=${questionContainer.data('question-id')}`, questionContainer.find('form'));
        if (response.fetchResponse == fetchResponses.SUCCESS) {
            questionContainer.find('div.answer .answer-content').html(await response.fetchPayload);

            questionContainer.find('div.answer').show();
            questionContainer.find('form').hide();
        }
        buttonUnload($(this));
     });

    $('body').on('click', '.editable-question .cancel-editing', function (e) {
        e.preventDefault();
        const questionContainer = $(this).closest('.editable-question');
        questionContainer.find('div.answer').show();
        questionContainer.find('form').hide();
    });


};