import { buttonLoad, buttonUnload } from "../shared/btn";
import { validatePostCode, hasNumber, validateFormData, validateEmail, validatePhoneNumber, scrollToFirstError } from "../shared/validator";

export function initaliseDetailStage() {
    validateForm($('#currentStep_Requestor_Firstname').length > 0 ? true : false)
    SetupAddressFinder();

}
var validateForm = function (validateRequestor) {
    
    $("form").on("submit", function (evt) {        
        if ($(document.activeElement).attr("id") == "btnBack") 
            return true;

        buttonLoad($("#btnNext"));
        
        const valid = validateFormData($(this), {
            "currentStep.Recipient.Firstname": (v) => (v.length >= 2 && !hasNumber(v)) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            "currentStep.Recipient.Lastname": (v) => (v.length >= 2 && !hasNumber(v)) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            "currentStep.Recipient.MobileNumber": (v, d) => {                
                if (!validateRequestor && (d["currentStep.Recipient.AlternatePhoneNumber"] == "" && v == "")) {                    
                    return "Please enter a mobile number or an alternative phone number"
                }
                return true;
            },
            "currentStep.Recipient.Email": (v) => {
                if (!validateRequestor && !validateEmail(v) || (v !== "" && !validateEmail(v))) {
                    return "Please enter a valid email address";
                }
                return true;
            },
            "currentStep.Recipient.Town": (v) => {
                if (v.length <= 2) {
                    $('.expander').slideDown();
                    return "Please enter a valid town / city";
                }
                return true;
            },
            "currentStep.Recipient.AddressLine1": (v) => {
                if (v.length <= 2) {
                    $('.expander').slideDown();
                    return "Please enter a valid first line of your address";
                }
                return true;
            },
            "currentStep.Recipient.Postcode": (v) => {
                if (v == "") {
                    $('.expander').slideDown();
                    return "Please enter a postcode";
                }
                return true;
            },                                            
            "currentStep.Requestor.Firstname": (v) => ((v.length >= 2 && !hasNumber(v))) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            "currentStep.Requestor.Lastname": (v) => ((v.length >= 2 && !hasNumber(v))) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            "currentStep.Requestor.MobileNumber": (v, d) => ((d["currentStep.Requestor.AlternatePhoneNumber"] !== "") || (v !== "")) || "Please enter a mobile number or an alternative phone number",
            "currentStep.Requestor.Email": (v) => (validateEmail(v)) ||  "Please enter a valid email address",
            "currentStep.Requestor.Postcode": (v) => (v != "") || "Please enter a postcode",
          
        });         
        
        runAdditionalValidation($(this)).then(function (additonalChecks) {
            evt.preventDefault();
            let validForm = (additonalChecks && valid);
            let postcodeValid =[];
            let postcodeInputs = $(".postcode-input");
            event.preventDefault(); //this will prevent the default submit needed now we do a call to api

            if (validForm) { // avoid calling service when possible, so check if the form is valid first
                //btnload
                postcodeInputs.each(function (i) {
                    var postcodeEl = $(this);
                    validatePostCode($(this).val()).then(function (response) {
                        postcodeValid.push(response);
                        if (!response) {
                            postcodeEl.next(".error").text("We could not validate that postcode, please check what you've entered and try again").show();
                            throw error("postcode val failed");
                        } else {
                            postcodeEl.next(".error").hide();
                        }
                        if (postcodeValid.length == (i + 1)) { // if we've checked all the postcodes, its now time to check their values and sumbit if they passed
                            validForm = (validForm && !postcodeValid.includes(false));
                            if (validForm) {
                                $('form').unbind('submit') // continue the submit unbind preventDefault
                                $('#btnNext').click();
                                postcodeEl.next(".error").hide();
                            } else {
                                postcodeEl.next(".error").text("We could not validate the postcode you've entered").show();
                                throw new error("postcode validation failed");
                            }
                        }
                    }).catch(function (e) {
                        console.error(e);
                        scrollToFirstError();
                        buttonUnload($("#btnNext"));
                    })
                })
            } else {
                scrollToFirstError();
                buttonUnload($("#btnNext"));
            }
        })
        
        return false;
    });
}


var  runAdditionalValidation = async function (form) {

    let mobile = form.find("input[name='currentStep.Recipient.MobileNumber']");
    let alt = form.find("input[name='currentStep.Recipient.AlternatePhoneNumber']");
    var v1 = validatePhoneNumber(mobile, "Please enter a valid mobile number starting with 07");
    var v2 = validatePhoneNumber(alt, "Please enter a valid alternative number");

    var mobileRequestor = form.find("input[name='currentStep.Requestor.MobileNumber']");
    var altRequestor = form.find("input[name='currentStep.Requestor.AlternatePhoneNumber']");

    if (mobileRequestor.length > 0) {
        var v3 = validatePhoneNumber(mobileRequestor, "Please enter a valid mobile number starting with 07");
        var v4 = validatePhoneNumber(altRequestor, "Please enter a valid alternative number");
        return (v1 && v2 && v3 && v4);
    }

    return (v1 && v2);

}

var SetupAddressFinder = function () {
    $(".manual_entry").on("click", function (evt) {
        evt.preventDefault();
        $(".expander").slideDown();
    });

    
    $("#address_finder").on("click", async function (evt) {
        
        evt.preventDefault();
        buttonLoad($(this));
        $("select[name=address_selector]").unbind("change");
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

                        $("input[name='currentStep.Recipient.AddressLine1']").val(address.addressLine1);
                        $("input[name='currentStep.Recipient.AddressLine2']").val(address.addressLine2);
                        $("input[name='currentStep.Recipient.Town']").val(address.locality);
                        $("input[name='currentStep.Recipient.Postcode']").val(address.postcode);             
                    });
                }
            }
        } catch (ex) {
            console.error(ex);
        }
        buttonUnload($(this));
    });
}