import { validateFormData, scrollToFirstError } from '../shared/validator'
import { initialiseGrowOnFocus } from '../ui/grow-on-focus'
import { initialiseTileSelector } from '../ui/tile-selector'
import { buttonLoad, buttonUnload } from '../shared/btn'
import { showServerSidePopup } from "../shared/popup";
import { hmsSubmit, fetchResponses } from "../shared/hmsFetch";

export function initialiseFeedbackCaptureForm() {

  initialiseGrowOnFocus();

  initialiseTileSelector();

  $("form").on("submit", function (event) {

    buttonLoad($("#btnSubmit"));

    const valid = validateFeedbackForm($(this));

    if (valid == false) {
      buttonUnload($("#btnSubmit"));;
      scrollToFirstError();
    }

    return valid;
  });
}

export async function showFeedbackPopup(jobId, role, successCallback) {
    const popupSource = `/api/feedback/get-post-task-feedback-popup?j=${jobId}&r=${role}`;

    let popup;
    const popupSettings = {
        acceptCallbackAsync: async () => {
            const form = $(popup).find('form');

            if (!validateFeedbackForm(form)) {
                return 'Please check your entries above and try again.';
            }

            var response = await hmsSubmit(`/api/feedback/put-feedback?j=${jobId}&r=${role}`, form);
            if (response.fetchResponse == fetchResponses.SUCCESS) {
                showServerSidePopup(`/api/feedback/get-feedback-thanks-popup`, {}, form);
                if (successCallback) {
                    successCallback();
                }
                return true;
            }
            return "Oops, we couldn't submit your feedback at the moment.";
        }
    };

    popup = await showServerSidePopup(popupSource, popupSettings);

    initialiseGrowOnFocus();

    initialiseTileSelector();
}

function validateFeedbackForm(form) {
  return validateFormData(form, {
    "FeedbackRating": (v) => (v != "") || "Please select an option"
  });
}