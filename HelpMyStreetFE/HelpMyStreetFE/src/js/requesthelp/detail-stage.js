import { buttonLoad, buttonUnload } from "../shared/btn";
import { validatePostCode } from "../shared/validator";

export var detailStage = {
    onBehalf: false,
    consentForContact: { val: false, errorSpan: "e-consentForContact" },
    yourDetails: {
        firstname: { val: null, errorSpan: "e-firstname_your" },
        lastname: { val: null, errorSpan: "e-lastname_your" },
        mobilenumber: { val: null, errorSpan: "e-mobile_number_your" },
        altnumber: { val: null, errorSpan: "e-alt_number_your" },
        email: { val: null, errorSpan: "e-email_your" },
        address: {
            addressLine1: { val: null, errorSpan: "e-addressline1_your" },
            addressLine2: { val: null, errorSpan: "e-addressline2_your" },
            locality: { val: null, errorSpan: "e-city_your" },
            county: { val: null, errorSpan: "e-county_your" },
            postcode: { val: null, errorSpan: "e-postcode_your" },
        }
    },
    theirDetails: {
        firstname: { val: null, errorSpan: "e-firstname_their" },
        lastname: { val: null, errorSpan: "e-lastname_their" },
        mobilenumber: { val: null, errorSpan: "e-mobile_number_their" },
        altnumber: { val: null, errorSpan: "e-alt_number_their" },
        email: { val: null, errorSpan: "e-email_their" },
        address: {
            addressLine1: { val: null, errorSpan: "e-addressline1_their" },
            addressLine2: { val: null, errorSpan: "e-addressline2_their" },
            locality: { val: null, errorSpan: "e-city_their" },
            county: { val: null, errorSpan: "e-county_their" },
            postcode: { val: null, errorSpan: "e-postcode_their" },
        }
    },

    validate: async function (requestFor) {
        let onBehalf = requestFor.val == "someone-else" ? true : false;
        $('.error').hide();
        let valid = await validateYourDetails(onBehalf);
        if (detailStage.consentForContact.val == false) {
            $('#' + detailStage.consentForContact.errorSpan).show().text("Please check to confirm that you have read and understood this");
            valid = false;
        }
        if (onBehalf == true) {
            if (await validateTheirDetails() == false) {
                valid = false;
            }
        }
        return valid;
    }
 }

export function initaliseDetailStage(requestFor) {
    detailStage.onBehalf = requestFor.val == "someone-else" ? true : false;
    $('#their-details').hide()
    intialiseConsentForContact();
    if (detailStage.onBehalf == true) {
        $('#their-details').show();
        initaliseAddressFinder("their", detailStage.theirDetails, false);
        initaliseAddressFinder("your", detailStage.yourDetails, true);
        HideorShowFindAddress(true, "your")
        intialiseFormFields("their", detailStage.theirDetails);
        intialiseFormFields("your", detailStage.yourDetails);        
    } else {
        HideorShowFindAddress(false, "your")
        initaliseAddressFinder("your", detailStage.yourDetails, false);
        intialiseFormFields("your", detailStage.yourDetails); 
    } 
}

var HideorShowFindAddress = function(hide, postfix){
    if (hide) {
        let inputClassforPostcodeSearch = $('input[name="postcode_search_' + postfix + '"]').parent();
        inputClassforPostcodeSearch.removeClass("sm3");
        inputClassforPostcodeSearch.addClass("sm6");
        let inputClassforAddressFine = $('#address_finder_' + postfix + '').parent();
        inputClassforAddressFine.addClass("dnone");
    } else {
        let inputClassforPostcodeSearch = $('input[name="postcode_search_' + postfix + '"]').parent();
        inputClassforPostcodeSearch.addClass("sm3");
        inputClassforPostcodeSearch.removeClass("sm6");
        let inputClassforAddressFine = $('#address_finder_' + postfix + '').parent();
        inputClassforAddressFine.removeClass("dnone");
    }



}

var intialiseFormFields = function (postfix, obj) {
    $('input[name="first_name_' + postfix + '"]').blur(function () {
        obj.firstname.val = $(this).val();
    });
    $('input[name="last_name_' + postfix + '"]').blur(function () {
        obj.lastname.val = $(this).val();
    });
    $('input[name="mobile_number_' + postfix + '"]').blur(function () {
        obj.mobilenumber.val = $(this).val();
    });
    $('input[name="alt_number_' + postfix + '"]').blur(function () {
        obj.altnumber.val = $(this).val();
    });
    $('input[name="email_' + postfix + '"]').blur(function () {
        obj.email.val = $(this).val();
    });

}

var initaliseAddressFinder = function (postfix, obj, bindPostcodeSearch) {

    $('input[name="address_line_1_' + postfix + '"]').blur(function () {
        obj.address.addressLine1.val = $(this).val();
    });
    $('input[name="address_line_2_' + postfix + '"]').blur(function () {
        obj.address.addressLine2.val = $(this).val();
    });
    $('input[name="city_' + postfix + '"]').blur(function () {
        obj.address.locality.val = $(this).val();
    });
    $('input[name="county_' + postfix + '"]').blur(function () {
        obj.address.county.val = $(this).val();
    });

    if (!bindPostcodeSearch) {
        $('input[name="postcode_' + postfix + '"]').blur(function () {
            obj.address.postcode.val = $(this).val();
        });
    } else {        
        $('input[name="postcode_search_' + postfix + '"]').blur(function () {
            obj.address.postcode.val = $(this).val();
        });
    }

    $('.manual-entry').click(function () {
        $("#expander_" + postfix).slideDown();
    })

    $("#address_finder_" + postfix).on("click", async function (evt) {
        evt.preventDefault();
        buttonLoad($(this));
        $("#address_selector_" + postfix).unbind("change");

        const postcode = $("input[name=postcode_search_" + postfix + "]").val();

        try {
            const resp = await fetch(`/api/postcode/${postcode}`);
            if (resp.ok) {
                const { hasContent, isSuccessful, content } = await resp.json();

                if (hasContent && isSuccessful) {
                    $("select[name=address_selector_" + postfix + "]").html(
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

                    $("#address_selector_" + postfix).slideDown();
                    $("select[name=address_selector_" + postfix + "]").on("change", function () {
                        const id = $(this).children("option:selected").val();
                        let address = content.addressDetails[id];
                        obj.address.addressLine1.val = address.addressLine1;                 
                        obj.address.addressLine2.val = address.addressLine2;
                        obj.address.locality.val = address.locality;
                        obj.address.postcode.val = address.postcode;

                        $(this).parent().find(".edit-address").show();
                        $("input[name=address_line_1_" + postfix + "]").val(obj.address.addressLine1.val);
                        $("input[name=address_line_2_" + postfix + "]").val(obj.address.addressLine2.val);
                        $("input[name=city_" + postfix + "]").val(obj.address.locality.val);
                        $("input[name=postcode_" + postfix + "]").val(obj.address.postcode.val);
                        //$("#expander_" + postfix).slideDown();                        
                    });
                }
            }
        } catch (ex) {
            console.error(ex);
        }
        buttonUnload($(this));
    });
}

var intialiseConsentForContact = function () {
    $('input[name="consent_for_contact"]').change(function () {
        detailStage.consentForContact = $(this).is(":checked");
    })
}


var validatePersonalDetails = function (obj) {
    let valid = true;
    if (!obj.firstname.val) {
        $('#' + obj.firstname.errorSpan).show().text("Please enter a first name");
        valid = false;
    }
    if (!obj.lastname.val) {
        $('#' + obj.lastname.errorSpan).show().text("Please enter a last name");
        valid = false;
    }
    if (!obj.mobilenumber.val && !obj.altnumber.val) {
        $('#' + obj.mobilenumber.errorSpan).show().text("Please enter either a mobile or an alternative phone number");
        valid = false;
    } else {
        if (!validPhoneNumber(obj.mobilenumber.val)) {
            $('#' + obj.mobilenumber.errorSpan).show().text("Please enter a valid UK mobile number");
            valid = false;
        }
        if (!validPhoneNumber(obj.altnumber.val)) {
            $('#' + obj.altnumber.errorSpan).show().text("Please enter a valid UK phone number");
            valid = false;
        }
    }
    return valid;
}

var validateAddress = async function (obj, expanderPostfix) {
    let valid = true;
    if (!obj.address.addressLine1.val) {
        $('#' + obj.address.addressLine1.errorSpan).show().text("Please enter a valid first line of your address");
        $("#expander_" + expanderPostfix).slideDown();
        valid = false;
    }
    if (!obj.address.locality.val) {
        $('#' + obj.address.locality.errorSpan).show().text("Please enter a valid city");
        $("#expander_" + expanderPostfix).slideDown();
        valid = false;
    }

    if (!obj.address.postcode.val) {
        $('#' + obj.address.postcode.errorSpan).show().text("Please enter a valid UK postcode");
        $("#expander_" + expanderPostfix).slideDown();
        valid = false;
    } else {
        buttonLoad($('#btnNext'))
        var postcodeValid = await validatePostCode(obj.address.postcode.val);
        if (!postcodeValid) {
            valid = false;
            $('#' + obj.address.postcode.errorSpan).show().text("Please enter a valid UK postcode");
            $("#expander_" + expanderPostfix).slideDown();
        }
        buttonUnload($('#btnNext'));
    }
    return valid;
}




var validateYourDetails = async function (onBehalf) {
    let valid = true;

    if (!validatePersonalDetails(detailStage.yourDetails))
        valid = false;

    if (!detailStage.yourDetails.email.val || !validEmail(detailStage.yourDetails.email.val)) {
        $('#' + detailStage.yourDetails.email.errorSpan).show().text("Please enter a valid email address");
        valid = false;
    }

    // if they have requested for somoene else, they dont need to find their address. they just need to enter a postcode
    if (onBehalf == false) {
        if (!await validateAddress(detailStage.yourDetails, "your"))
            valid = false;
    } else {
        if (!await validatePostCode(detailStage.yourDetails.address.postcode.val)) {
            valid = false;
            $('#e-postcode_search_your').show().text("Please enter a valid UK postcode");
        }
    }

    return valid;
}

var validateTheirDetails = async function () {
    let valid = true;
    if (!validatePersonalDetails(detailStage.theirDetails))
        valid = false;


    if (detailStage.theirDetails.email.val && !validEmail(detailStage.theirDetails.email.val)) {
        $('#' + detailStage.theirDetails.email.errorSpan).show().text("Please enter a valid email address");
        valid = false;
    }


    if (!await validateAddress(detailStage.theirDetails, "their"))
        valid = false;

    return valid;

}

var validEmail = function (email) {
    let re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

var validPhoneNumber = function (phoneNumber) {
    if (!phoneNumber) return true;
    return ((phoneNumber.replace(" ", "").length === 10 || phoneNumber.replace(" ", "").length === 11) && phoneNumber[0] === "0");
}