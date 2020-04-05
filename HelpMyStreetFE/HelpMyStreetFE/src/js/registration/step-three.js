import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData } from "../shared/validator";

export function initialiseStepThree() {
  $("#registration_form").on("submit", function () {
    let valid = true;
    const obj = $(this)
      .serializeArray()
      .reduce((acc, cur) => {
        acc[cur.name] = cur.value;

        return acc;
      }, {});

    $(".error").hide();

    if (!obj["volunteer[]"]) {
      console.log('showing');
      valid = false;
      $("#volunteer-error").show();
    }

    if (!obj["volunteer_distance"]) {
      valid = false;
      $("#distance-error").show();
    }

    if (!obj["volunteer_phone_contact"]) {
      valid = false;
      $("#contact-error").show();
    }

    if (!obj["volunteer_medical_condition"]) {
      valid = false;
      $("#medical-error").show();
    }

    return valid;
  });
}
