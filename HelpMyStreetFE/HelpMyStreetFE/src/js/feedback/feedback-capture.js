import { validateFormData, scrollToFirstError } from '../shared/validator'
import { initialiseGrowOnFocus } from '../ui/grow-on-focus'
import { initialiseTileSelector } from '../ui/tile-selector'
import { buttonLoad, buttonUnload } from '../shared/btn'
import { showServerSidePopup } from "../shared/popup";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch";

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

export async function showFeedbackPopup(jobId, role) {
  const popupSource = `/api/feedback/get-post-task-feedback-popup?j=${jobId}&r=${role}`;

  let popup;
  const popupSettings = {
    acceptCallbackAsync: async () => {
      const form = $(popup).find('form');

      if (!validateFeedbackForm(form)) {
        return 'Please check your entries above and try again.';
      }

      const formData = $(form).serializeArray();
      let dataToSend = {};
      formData.forEach((d) => {
        if ($(form).find(`input[name="${d.name}"]`).attr('type') == 'number') {
          dataToSend[d.name] = parseFloat(d.value);
        } else {
          dataToSend[d.name] = d.value;
        }
      });

      var fetchRequestData = {
        method: 'POST',
        body: JSON.stringify(dataToSend),
        headers: { 'Content-Type': 'application/json' },
      };
      var response = await hmsFetch(`/api/feedback/put-feedback?j=${jobId}&r=${role}`, fetchRequestData);
      if (response.fetchResponse == fetchResponses.SUCCESS) {
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
    "FeedbackRating": (v) => (!isNaN(v) && v > 0) || "Please select an option"
  });
}