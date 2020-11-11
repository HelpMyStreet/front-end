import { validateFormData, scrollToFirstError } from '../shared/validator'
import { initialiseGrowOnFocus } from '../ui/grow-on-focus'
import { initialiseTileSelector } from '../ui/tile-selector'
import { buttonLoad, buttonUnload } from '../shared/btn'

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

export function validateFeedbackForm(form) {
  return validateFormData(form, {
    "FeedbackRating": (v) => (!isNaN(v) && v > 0) || "Please select an option"
  });
}