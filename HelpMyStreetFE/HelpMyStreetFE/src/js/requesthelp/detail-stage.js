import { buttonLoad, buttonUnload } from "../shared/btn";
import { validatePostCode, hasNumber, validateFormData, validateEmail, validatePhoneNumber, scrollToFirstError } from "../shared/validator";
import { trackEvent } from "../shared/tracking-helper";
import { loadQuestions, validateQuestions } from "./requesthelp-shared.js";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";
import { filterInput, inputTypes } from "../shared/input-filter";

export function initaliseDetailStage() {
    validateForm($('#currentStep_Type').val() == 'Myself' ? true : false);
    SetupAddressFinder();
    filterInput($('input[inputmode="numeric"]'), inputTypes.NUMERIC);

    trackEvent("Request form", "View 1.details", "", 0);
}

var validateForm = function (validateRecipientAsRequestor) {
    
    $("form").on("submit", function (evt) {        
        if ($(document.activeElement).attr("id") == "btnBack") {
            trackEvent("Request form", "Click Back", "", 0);
            return true;
        }

        buttonLoad($("#btnNext"));
        const valid = validateQuestions() && validateFormData($(this), {
            "currentStep.Recipient.Firstname": (v) => v.length >= 2 && !hasNumber(v) || "Please enter a name with at least 2 letters and no numbers",
            "currentStep.Recipient.Lastname": (v) => v.length >= 2 && !hasNumber(v) || "Please enter a name with at least 2 letters and no numbers",
            "currentStep.Recipient.MobileNumber": (v) => v != "" || "Please enter a phone number",
            "currentStep.Recipient.Email": (v) => {
              if (validateRecipientAsRequestor && !validateEmail(v) || (v !== "" && !validateEmail(v))) {
                    return "a valid email address";
                }
                return true;
            },
            "currentStep.Recipient.Town": (v) => {
                if (v.length <= 2) {
                    $('.expander').slideDown();
                    return "a valid town / city";
                }
                return true;
            },
            "currentStep.Recipient.AddressLine1": (v) => {
                if (v.length <= 2) {
                    $('.expander').slideDown();
                    return "a valid first line of your address";
                }
                return true;
            },
            "currentStep.Recipient.Postcode": (v) => {
                if (v == "") {
                    $('.expander').slideDown();
                    return "a postcode";
                }
                return true;
            },                                            
            "currentStep.Requestor.Firstname": (v) => v.length >= 2 && !hasNumber(v) || "Please enter a name with at least 2 letters and no numbers",
            "currentStep.Requestor.Lastname": (v) => v.length >= 2 && !hasNumber(v) || "Please enter a name with at least 2 letters and no numbers",
            "currentStep.Requestor.MobileNumber": (v) => v !== "" || "Please enter a mobile number or an alternative phone number",
            "currentStep.Requestor.Email": (v) => (validateEmail(v)) ||  "Please enter a valid email address",
            "currentStep.Requestor.Postcode": (v) => (v != "") || "Please enter a postcode",
          
        });   
        
        runAdditionalValidation($(this)).then(async function (additonalChecks) {
            evt.preventDefault();
            let validForm = (additonalChecks && valid);
            event.preventDefault(); //this will prevent the default submit needed now we do a call to api
            // avoid calling service when possible, so check if the form is valid first

            trackEvent("Request form", "Submit 1.details", validForm ? "(Valid)" : "(Invalid)", 0);

            if (!validForm) {
                scrollToFirstError();
                buttonUnload($("#btnNext"));
                return false;
            }        
            try {
                let postcodeInputs = $(".postcode-input");    
                await validatePostcodeInputs(postcodeInputs, 0); // recursive function, if postcode is invalid it throws an error
                $('form').unbind('submit') // continue the submit unbind preventDefault
                $('#btnNext').click();
            } catch (e) {
                console.error(e);
                scrollToFirstError();
                buttonUnload($("#btnNext"));
            }
        });             
    });
}


var  runAdditionalValidation = async function (form) {

    let mobile = form.find("input[name='currentStep.Recipient.MobileNumber']");
    let alt = form.find("input[name='currentStep.Recipient.AlternatePhoneNumber']");
    var v1 = validatePhoneNumber(mobile, "Please enter a valid mobile number", true);
    var v2 = validatePhoneNumber(alt, "Please enter a valid alternative number");

    var mobileRequestor = form.find("input[name='currentStep.Requestor.MobileNumber']");
    var altRequestor = form.find("input[name='currentStep.Requestor.AlternatePhoneNumber']");

    if (mobileRequestor.length > 0) {
        var v3 = validatePhoneNumber(mobileRequestor, "Please enter a valid mobile number", true);
        var v4 = validatePhoneNumber(altRequestor, "Please enter a valid alternative number");
        return (v1 && v2 && v3 && v4);
    }

    return (v1 && v2);

}

var SetupAddressFinder = function () {
    $(".manual_entry").on("click", function (evt) {
        trackEvent("Request form", "Click Manual entry");

        evt.preventDefault();
        $(".expander").slideDown();
        $(".manual_entry").hide();
    });

    
    $("#address_finder").on("click", async function (evt) {
        trackEvent("Request form", "Click Find address");

 
        evt.preventDefault();
        buttonLoad($(this));
        $("select[name=address_selector]").unbind("change");
        const postcode = $("input[name=postcode_search]").val();
        
        try {
            if (postcode == "") {
                let postcodeInput = $("input[name='postcode_search']");
                postcodeInput.find("~ .error").text("Please enter a postcode to search").show();
                buttonUnload($(this));
                return;
            }
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
                        $("input[name='currentStep.Recipient.AddressLine1']").val(address.addressLine1);
                        $("input[name='currentStep.Recipient.AddressLine2']").val(address.addressLine2);
                        $("input[name='currentStep.Recipient.Town']").val(address.locality);
                        $("input[name='currentStep.Recipient.Postcode']").val(address.postcode);
                    });
                } else {
                    let postcodeInput = $("input[name='postcode_search']");
                    postcodeInput.find("~ .error").text("We could not validate that postcode, please check what you've entered and try again").show();
                }
            } else {
                let postcodeInput = $("input[name='postcode_search']");
                postcodeInput.find("~ .error").text("We could not validate that postcode, please check what you've entered and try again").show();
            }
        } catch (ex) {
            console.error(ex);
        }
        buttonUnload($(this));
    });
}


async function validatePostcodeInputs(arr, i) {     
    if (i == arr.length) return false;    
    var postcode = arr.eq(i);
    let response = await validatePostCode(postcode.val());    
     if (!response) {
          postcode.next(".error").text("We could not validate that postcode, please check what you've entered and try again").show();
          throw new Error("Postcode validation failed");
        } else {
           postcode.next(".error").hide();
    }

    i++;    
    await validatePostcodeInputs(arr, i);
}