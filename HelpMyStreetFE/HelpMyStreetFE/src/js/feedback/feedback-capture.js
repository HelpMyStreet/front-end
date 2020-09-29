import { validateFormData, scrollToFirstError } from '../shared/validator'
import { initialiseGrowOnFocus } from '../ui/grow-on-focus'
import { initialiseTileSelector } from '../ui/tile-selector'
import { buttonLoad, buttonUnload } from '../shared/btn'

export function initialiseFeedbackCaptureForm() {

  initialiseGrowOnFocus();

  initialiseTileSelector();

  $("form").on("submit", function (event) {

    buttonLoad($("#btnSubmit"));

    const valid = validateFormData($(this), {
      "FeedbackRating": (v) => (!isNaN(v) && v > 0) || "Please select an option"
    });

    if (valid == false) {
      buttonUnload($("#btnSubmit"));;
      scrollToFirstError();
    }

    return valid;
  });
}

