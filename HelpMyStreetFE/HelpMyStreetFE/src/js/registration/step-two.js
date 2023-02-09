import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData, validatePostCode, validatePhoneNumber, hasNumber } from "../shared/validator";
import { datepickerLoad, validateDate, dateValidationSchemes } from "../shared/date-picker";
import { trackEvent } from "../shared/tracking-helper";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";
import { filterInput, inputTypes } from "../shared/input-filter";

export function initialiseStepTwo() {

  trackEvent("Registration flow", "View Step 2");
  filterInput($('input[inputmode="numeric"]'), inputTypes.NUMERIC);

  $(".manual_entry").on("click", function (evt) {
    evt.preventDefault();
    $(".expander").slideDown();
    $(".manual_entry").hide();
    trackEvent("Registration flow", "Click Manual entry");
 });

  datepickerLoad($('#datepicker'), $('#datepicker-error'), dateValidationSchemes.OVER_18);
  $("#address_finder").on("click", async function (evt) {
    evt.preventDefault();
    buttonLoad($(this));
    $("#address_selector").unbind("change");
    trackEvent("Registration flow", "Click Find address");

    const postcode = $("input[name=postcode_search]").val();
    try {
        const resp = await hmsFetch(`/api/postcode/${postcode}`);
        if (resp.fetchResponse == fetchResponses.SUCCESS) {
            const { hasContent, isSuccessful, content } = await resp.fetchPayload;

            if (hasContent && isSuccessful) {
                let postcodeInput = $("input[name='postcode_search']");
                postcodeInput.find("~ .error").hide();

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
                $("#address_finder ~ .manual_entry").hide();
                $("select[name=address_selector]").on("change", function () {
                    const id = $(this).children("option:selected").val();

                    const address = content.addressDetails[id];

                    $("input[name=address_line_1]").val(address.addressLine1);
                    $("input[name=address_line_2]").val(address.addressLine2);
                    $("input[name=city]").val(address.locality);
                    $("input[name=postcode]").val(address.postcode);

                    $("select[name='address_selector']").find("~ .error").hide();

                    if (address.addressLine1.length <= 2 || address.locality.length <= 2) {
                        // Address will fail validation
                        $(".expander").slideDown();
                        $(".manual_entry").hide();
                    }
                });
            } else {
                let postcodeInput = $("input[name='postcode_search']");
                postcodeInput.find("~ .error").text("We could not validate that postcode, please check what you've entered and try again").show();
            }
        } else {
            let postcodeInput = $("input[name='postcode_search']");
            postcodeInput.find("~ .error").text("Sorry, there's been an error. Please try again.").show();
        }
    } catch (ex) {
      console.error(ex);
    }
    buttonUnload($(this));
  });

    $("#registration_form").on("submit", function (event) {
        const valid = validateFormData($(this), {
            first_name: (v) => (v.length >= 2 && !hasNumber(v)) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            last_name: (v) => (v.length >= 2 && !hasNumber(v)) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            postcode: (v) => v !== "" ||
                "Please enter a postcode",
            dob: (v) => v !== "" || "Please enter a valid date of birth",
            alt_number: (v, d) =>
                ((d.mobile_number !== "") || (v !== "")) || "Please enter a mobile number or an alternative phone number",
            city: (v) =>
                (v.length > 2) ||
                "Please enter a valid city",
            address_line_1: (v) =>
                (v.length > 2) ||
                "Please enter a valid first line of your address",
        });

        runAdditionalValidation($(this)).then(function (additonalChecks) {
            let validForm = (additonalChecks && valid);            
            let postcodeValid;
            let postcodeInput = $("input[name='postcode']");

            if (!valid && $(".expander:visible").length == 0 && $("input[name='address_line_1']").val().length <= 2) {
                // Address invalid and manual entry fields not visible
                let postcodeSearchInput = $("input[name='postcode_search']");
                postcodeSearchInput.find("~ .error").hide();
                if ($("#address_selector:visible").length == 0) {
                    if (postcodeSearchInput.val().length < 5) {
                        postcodeSearchInput.find("~ .error").text("Please enter your address").show();
                    } else {
                        postcodeSearchInput.find("~ .error").text("Please confirm your address").show();
                    }
                } else {
                    $("select[name='address_selector']").find("~ .error").text("Please select your address").show();
                }
            }


            event.preventDefault(); //this will prevent the default submit needed now we do a call to api
            trackEvent("Registration flow", "Submit Step 2", validForm ? "(Valid)" : "(Invalid)");
            if (validForm) { // avoid calling service when possible, so check if the form is valid first
                buttonLoad($('#submit_button'));
                validatePostCode(postcodeInput.val()).then(function (response) {
                    postcodeValid = response;
                    if (!postcodeValid) {
                        postcodeInput.find("~ .error").text("We could not validate that postcode, please check what you've entered and try again").show();
                        buttonUnload($('#submit_button'));
                    } else {
                        postcodeInput.find("~ .error").hide();
                    }
                }).then(function () {
                    validForm = (validForm && postcodeValid);
                    if (validForm) {
                        $("#registration_form").unbind('submit').submit(); // continue the submit unbind preventDefault
                    }
                }).catch(function () {
                    buttonUnload($('#submit_button'));
                });

            }
        })
  });
}



var runAdditionalValidation = async function(form) {
    let dob = form.find("input[name='dob']");    
    let mobile = form.find("input[name='mobile_number']");
    let alt = form.find("input[name='alt_number']");         

    var v1 = validateDate(dob.val(), $(dob).find('~ .error'), dateValidationSchemes.OVER_18);
    var v2 = validatePhoneNumber(mobile, "Please enter a valid mobile number starting with 07", true);
    var v3 = validatePhoneNumber(alt, "Please enter a valid alternative number");
    return (v1 && v2 && v3);
}