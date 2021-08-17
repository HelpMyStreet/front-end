

export async function initialiseEditableQuestions() {
    $('body').on('click', '.editable-question .edit', function (e) {
        e.preventDefault();
        const questionContainer = $(this).closest('.editable-question');
        questionContainer.find('div.answer').hide();
        questionContainer.find('form').show();
    });

    $('body').on('click', '.editable-question .save-changes', function (e) {
        e.preventDefault();
        const questionContainer = $(this).closest('.editable-question');
        questionContainer.find('div.answer').show();
        questionContainer.find('form').hide();
    });

    $('body').on('click', '.editable-question .cancel-editing', function (e) {
        e.preventDefault();
        const questionContainer = $(this).closest('.editable-question');
        questionContainer.find('div.answer').show();
        questionContainer.find('form').hide();
    });


};