import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData, validatePostCode, validatePhoneNumber, hasNumber } from "../shared/validator";
import { datepickerLoad, validateDob } from "../shared/date-picker";
import { trackEvent } from "../shared/tracking-helper";
import "isomorphic-fetch";

export function initialiseStepTwo() {

  trackEvent("Registration flow", "View Step 2");

  $("#manual_address").on("click", function (evt) {
    evt.preventDefault();
    $(".expander").slideDown();
    trackEvent("Registration flow", "Click Manual entry");
 });

  datepickerLoad('datepicker');
  $("#address_finder").on("click", async function (evt) {
    evt.preventDefault();
    buttonLoad($(this));
    $("#address_selector").unbind("change");
    trackEvent("Registration flow", "Click Find address");

    const postcode = $("input[name=postcode_search]").val();
    try {
        const resp = await fetch(`/api/postcode/${postcode}`);
        if (resp.ok) {
            const { hasContent, isSuccessful, content } = await resp.json();

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
                $("select[name=address_selector]").on("change", function () {
                    const id = $(this).children("option:selected").val();

                    const address = content.addressDetails[id];

                    $("input[name=address_line_1]").val(address.addressLine1);
                    $("input[name=address_line_2]").val(address.addressLine2);
                    $("input[name=city]").val(address.locality);
                    $("input[name=postcode]").val(address.postcode);
                    $(".expander").slideDown();
                });
            } else {
                let postcodeInput = $("input[name='postcode_search']");
                postcodeInput.find("~ .error").text("We could not validate that postcode, please check what you've entered and try again").show();
            }
        } 
    } catch (ex) {
      console.error(ex);
    }
    buttonUnload($(this));
  });

    $("#registration_form").on("submit", function (event) {
        $(".expander").slideDown();        
        const valid = validateFormData($(this), {
            first_name: (v) => (v.length >= 2 && !hasNumber(v)) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            last_name: (v) => (v.length >= 2 && !hasNumber(v)) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            postcode: (v) => v !== "" ||
                "Please enter a postcode",
            dob: (v) => v !== "" || "Please enter a valid date of birth",          
            alt_number: (v, d) =>
                ((d.mobile_number !== "") || (v !== ""))  || "Please enter a mobile number or an alternative phone number",
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

    var v1 = validateDob(dob.val(), dob.attr('id'));
    var v2 = validatePhoneNumber(mobile, "Please enter a valid mobile number starting with 07");
    var v3 = validatePhoneNumber(alt, "Please enter a valid alternative number");
    return (v1 && v2 && v3);
}