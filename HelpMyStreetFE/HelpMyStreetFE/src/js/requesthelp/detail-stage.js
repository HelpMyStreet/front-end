import { buttonLoad, buttonUnload } from "../shared/btn";
import { validatePostCode, hasNumber, validateFormData, validateEmail, validatePhoneNumber } from "../shared/validator";

export function initaliseDetailStage() {
    validateForm($('#currentStep_Requestor_Firstname').length > 0 ? true : false)
    SetupAddressFinder();

}
var validateForm = function (validateRequestor) {
    
    $("form").on("submit", function (evt) {
       
        const valid = validateFormData($(this), {
            "currentStep.Recipient.Firstname": (v) => (v.length >= 2 && !hasNumber(v)) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            "currentStep.Recipient.Lastname": (v) => (v.length >= 2 && !hasNumber(v)) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            "currentStep.Recipient.MobileNumber": (v, d) => ((d["currentStep.Recipient.AlternatePhoneNumber"] !== "") || (v !== "")) || "Please enter a mobile number or an alternative phone number",
            "currentStep.Recipient.Email": (v) => {
                if (!validateRequestor && !validateEmail(v) || (v !== "" && !validateEmail(v))) {
                    return "Please enter a valid email address";                                        
                }     
                return true;
            },
            "currentStep.Recipient.Town": (v) =>
                (v.length > 2) ||
                "Please enter a valid town / city",
            "currentStep.Recipient.AddressLine1": (v) =>
                (v.length > 2) ||
                "Please enter a valid first line of your address",
            "currentStep.Recipient.Postcode": (v) => (v !== "") || "Please enter a postcode",


            "currentStep.Requestor.Firstname": (v) => ((v.length >= 2 && !hasNumber(v))) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            "currentStep.Requestor.Lastname": (v) => ((v.length >= 2 && !hasNumber(v))) || "Please enter a name of at least 2 characters (letters and common punctuation marks only)",
            "currentStep.Requestor.MobileNumber": (v, d) => ((d["currentStep.Requestor.AlternatePhoneNumber"] !== "") || (v !== "")) || "Please enter a mobile number or an alternative phone number",
            "currentStep.Requestor.Email": (v) => (validateEmail(v)) ||  "Please enter a valid email address",
            "currentStep.Requestor.Postcode": (v) => (v != "") || "Please enter a postcode"

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
                            //btn unload
                        } else {
                            postcodeEl.next(".error").hide();
                        }                        
                        if (postcodeValid.length == (i + 1)) { // if we've checked all the postcodes, its now time to check their values and sumbit if they passed
                            validForm = (validForm && !postcodeValid.includes(false));
                            
                            if (validForm) {
                                $('form').unbind('submit').submit(); // continue the submit unbind preventDefault
                            }
                        }
                    }).catch(function () {
                      //btn unload
                    })
                })             
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
        console.log(postcode);
        try {
            const resp = await fetch(`/api/postcode/${postcode}`);
            if (resp.ok) {
                const { hasContent, isSuccessful, content } = await resp.json();

                if (hasContent && isSuccessful) {
                    console.log("success");
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