import { buttonLoad, buttonUnload } from "../shared/btn";
export var detailStage = new Object();
detailStage.onBehalf = false
detailStage.consentForContact = { val: false, errorSpan: "e-consentForContact" };
detailStage.yourDetails = {
    firstname: { val: null, errorSpan: "e-firstname_your"},
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
}
detailStage.theirDetails = {
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
}


detailStage.validate = function (requestFor) {
    var onBehalf = requestFor.val == "someone-else" ? true : false;
    $('.error').hide();
    var valid = validateYourDetails();
    if (detailStage.consentForContact.val == false) {
        $('#' + detailStage.consentForContact.errorSpan).show().text("Please check to confirm that you have read and understood this");
        valid = false;
    }
    if (onBehalf == true) {
        if (validateTheirDetails() == false) {
            valid = false;
        }
    }
    return valid;

}

var validateYourDetails = function () {
    var valid = true;
    if (!detailStage.yourDetails.firstname.val) {
        $('#' + detailStage.yourDetails.firstname.errorSpan).show().text("Please enter a first name");
        valid = false;
    }
    if (!detailStage.yourDetails.lastname.val) {
        $('#' + detailStage.yourDetails.lastname.errorSpan).show().text("Please enter a last name");
        valid = false;
    }
    if (!detailStage.yourDetails.mobilenumber.val && !detailStage.yourDetails.altnumber.val) {
        $('#' + detailStage.yourDetails.mobilenumber.errorSpan).show().text("Please enter either a mobile or an alternative phone number");
        valid = false;
    } else {
        if (!validPhoneNumber(detailStage.yourDetails.mobilenumber.val)) {
            $('#' + detailStage.yourDetails.mobilenumber.errorSpan).show().text("Please enter a valid UK mobile number");
        }
        if (!validPhoneNumber(detailStage.yourDetails.altnumber.val)) {
            $('#' + detailStage.yourDetails.altnumber.errorSpan).show().text("Please enter a valid UK phone number");
        }
    }

    if (!detailStage.yourDetails.email.val || !validEmail(detailStage.yourDetails.email.val)) {
        $('#' + detailStage.yourDetails.email.errorSpan).show().text("Please enter a valid email address");        
        valid = false;
    }

    if (!detailStage.yourDetails.address.addressLine1.val) {
        $('#' + detailStage.yourDetails.address.addressLine1.errorSpan).show().text("Please enter a valid first line of your address");
        $("#expander_your").slideDown();
        valid = false;
    }
    if (!detailStage.yourDetails.address.locality.val) {
        $('#' + detailStage.yourDetails.address.locality.errorSpan).show().text("Please enter a valid city");
        $("#expander_your").slideDown();
        valid = false;
    }

    if (!detailStage.yourDetails.address.postcode.val) {
        $('#' + detailStage.yourDetails.address.postcode.errorSpan).show().text("Please enter a valid UK postcode");
        $("#expander_your").slideDown();
        valid = false;
    }

    return valid;

    
}

var validateTheirDetails = function () {
    var valid = true;
    if (!detailStage.theirDetails.firstname.val) {
        $('#' + detailStage.theirDetails.firstname.errorSpan).show().text("Please enter a first name");
        valid = false;
    }
    if (!detailStage.theirDetails.lastname.val) {
        $('#' + detailStage.theirDetails.lastname.errorSpan).show().text("Please enter a last name");
        valid = false;
    }
    if (!detailStage.theirDetails.mobilenumber.val && !detailStage.theirDetails.altnumber.val) {
        $('#' + detailStage.theirDetails.mobilenumber.errorSpan).show().text("Please enter either a mobile or an alternative phone number");
        valid = false;
    } else {
        if (!validPhoneNumber(detailStage.theirDetails.mobilenumber.val)) {
            $('#' + detailStage.theirDetails.mobilenumber.errorSpan).show().text("Please enter a valid UK mobile number");
        }
        if (!validPhoneNumber(detailStage.theirDetails.altnumber.val)) {
            $('#' + detailStage.theirDetails.altnumber.errorSpan).show().text("Please enter a valid UK phone number");
        }
    }

    if (!detailStage.theirDetails.address.addressLine1.val) {
        $('#' + detailStage.theirDetails.address.addressLine1.errorSpan).show().text("Please enter a valid first line of your address");
        $("#expander_their").slideDown();
        valid = false;
    }
    if (!detailStage.theirDetails.address.locality.val) {
        $('#' + detailStage.theirDetails.address.locality.errorSpan).show().text("Please enter a valid city");
        $("#expander_their").slideDown();
        valid = false;
    }

    if (!detailStage.theirDetails.address.postcode.val) {
        $('#' + detailStage.theirDetails.address.postcode.errorSpan).show().text("Please enter a valid UK postcode");
        $("#expander_their").slideDown();
        valid = false;
    }

    return valid;


}


var validEmail = function(email){    
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;    
    return re.test(email);
}

var validPhoneNumber = function (phoneNumber) {
    if (!phoneNumber) return true;
    return ((phoneNumber.replace(" ", "").length === 10 || phoneNumber.replace(" ", "").length === 11) && phoneNumber[0] === "0");
}

export function initaliseDetailStage(requestFor) {
    detailStage.onBehalf = requestFor.val == "someone-else" ? true : false;
    $('#their-details').hide()
    intialiseConsentForContact();
    if (detailStage.onBehalf == true) {
        $('#their-details').show();
        initaliseAddressFinder("their", detailStage.theirDetails);
        initaliseAddressFinder("your", detailStage.yourDetails);
        intialiseFormFields("their", detailStage.theirDetails);
        intialiseFormFields("your", detailStage.yourDetails);        
    } else {
        initaliseAddressFinder("your", detailStage.yourDetails);
        intialiseFormFields("your", detailStage.yourDetails); 
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

var initaliseAddressFinder = function (postfix, obj) {

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
    $('input[name="postcode_' + postfix + '"]').blur(function () {
        obj.address.postcode.val = $(this).val();
    });

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
                        var address = content.addressDetails[id];
                        obj.address.addressLine1.val = address.addressLine1;                 
                        obj.address.addressLine2.val = address.addressLine2;
                        obj.address.locality.val = address.locality;
                        obj.address.postcode.val = address.postcode;
                        

                        $("input[name=address_line_1_" + postfix + "]").val(obj.address.addressLine1.val);
                        $("input[name=address_line_2_" + postfix + "]").val(obj.address.addressLine2.val);
                        $("input[name=city_" + postfix + "]").val(obj.address.locality.val);
                        $("input[name=postcode_" + postfix + "]").val(obj.address.postcode.val);
                        $("#expander_" + postfix).slideDown();                        
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