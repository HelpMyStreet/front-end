import { buttonLoad, buttonUnload } from "../shared/btn";
import { hmsSubmit, fetchResponses } from "../shared/hmsFetch";

export async function initialiseEditableBiography() {
    $('body').on('click', '.editable-biography .edit', function (e) {
        e.preventDefault();
        const questionContainer = $(this).closest('.editable-biography');
        questionContainer.find('div.answer').hide();
        questionContainer.find('form').show();
    });

    $('body').on('click', '.editable-biography .save-changes', async function (e) {
        e.preventDefault();
        buttonLoad($(this));
        const biographyContainer = $(this).closest('.editable-biography');

        const response = await hmsSubmit(`/api/volunteers/update-biography`, biographyContainer.find('form'));
        if (response.fetchResponse == fetchResponses.SUCCESS) {
            biographyContainer.find('div.answer .training__answer-content').html(await response.fetchPayload);
            biographyContainer.find('div.answer').show();
            biographyContainer.find('form').hide();
        }
        buttonUnload($(this));
     });

    $('body').on('click', '.editable-biography .cancel-editing', function (e) {
        e.preventDefault();
        const biographyContainer = $(this).closest('.editable-biography');
        biographyContainer.find('div.answer').show();
        biographyContainer.find('form').hide();
    });


};