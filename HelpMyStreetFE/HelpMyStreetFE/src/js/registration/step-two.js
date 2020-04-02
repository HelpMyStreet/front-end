import { buttonLoad, buttonUnload } from "../shared/btn";

function validateFormData(form, validation) {
  return form
    .find(".input")
    .get()
    .reduce((acc, cur) => {
      const inp = $(cur).find("input");
      if (inp.length) {
        const { name, value } = inp[0];

        const errDisplay = $(cur).find(".error");
        errDisplay && errDisplay.text("").hide();

        const validator = validation[name];

        if (validator) {
          const valid = validator(value);
          if (valid !== true) {
            acc = false;
            errDisplay.text(valid).show();
          }
        }
      }

      return acc;
    }, true);
}

export function initialiseStepTwo() {
  $("#manual_address").on("click", function (evt) {
    evt.preventDefault();
    $(".expander").slideDown();
  });

  $("#address_finder").on("click", async function (evt) {
    evt.preventDefault();
    buttonLoad($(this));
    $("#address_selector").unbind("change");

    const postcode = $("input[name=postcode_search]").val();

    try {
      const resp = await fetch(`/api/postcode/${postcode}`);
      if (resp.ok) {
        const { hasContent, isSuccessful, content } = await resp.json();

        if (hasContent && isSuccessful) {
          $("select[name=address_selector]").html(
            content.addressDetails.reduce((acc, cur, i) => {
              const text = Object.keys(cur).reduce((tAcc, tCur) => {
                if (cur[tCur] != null) {
                  tAcc += tAcc === "" ? "" : ", ";
                  tAcc += `${cur[tCur]}`;
                }

                return tAcc;
              }, "");

              acc += `<option value="${i}">${text}</option>`;
              return acc;
            }, '<option value="" selected disabled hidden>Choose here</option>')
          );

          $("#address_selector").slideDown();
          $("select[name=address_selector]").on("change", function () {
            const id = $(this).children("option:selected").val();

            const address = content.addressDetails[id];

            $("input[name=address_line_1]").val(address.addressLine1);
            $("input[name=address_line_2]").val(address.addressLine2);
            $("input[name=city]").val(address.locality);
            $("input[name=postcode]").val(address.postcode);
            $(".expander").slideDown();
          });
        }
      }
    } catch (ex) {
      console.error(ex);
    }
    buttonUnload($(this));
  });

  $("#registration_form").on("submit", function () {
    $(".expander").slideDown();
    const valid = validateFormData($(this), {
      first_name: (v) => v != "" || "Please enter a first name",
      last_name: (v) => v != "" || "Please enter a last name",
      postcode: (v) => v != "" || "Please enter a postcode",
      mobile_number: (v) =>
        v.length === 11 || "Please enter a valid mobile number",
      alt_number: (v) =>
        v == "" || v.length === 11 || "Please enter a valid phone number",
      address_line_1: (v) =>
        v != "" || "Please enter the first line of your address",
    });

    return valid;
  });
}
