import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";

export function validateFormData(form, validation) {
  const obj = form
    .find(".input")
    .get()
    .reduce((acc, cur) => {
      const inp = $(cur).find("input");
      if (inp[0]) {
        const { name, value, type } = inp[0];
        acc[name] = (type === "checkbox" && type !== "radio") ? inp.is(":checked") : value;
        acc[name] = (type === "radio" && !inp.is(":checked")) ? undefined : acc[name];
      } else {
        const ta = $(cur).find("textarea");
        if (ta[0]) {
          acc[ta[0].name] = ta.val();
        }
      }

      return acc;
    }, {});

  return Object.entries(obj).reduce((acc, cur) => {
    const [name, value] = cur;

    const errDisplay = $(`input[name="${name}"] ~ .error, textarea[name="${name}"] ~ .error`);

    const isRequired = $(`input[name="${name}"]`).data("required") != "False";
    const hasGroupData = $(`input[name="${name}"]`).data("validationgroup");
    const minimumGroupValidations = $(`input[name="${name}"]`).data("minimumvalidations") ?? 1;

    errDisplay && errDisplay.text("").hide();

    const validator = validation[name];
    if (validator) {
      if (!isRequired && hasGroupData != undefined){
        let otherGroupMembers = $.makeArray($(`[data-validationgroup='${hasGroupData}']`));
        let result = otherGroupMembers.reduce((acc, cur) => {
          acc = validator($(cur).val(), obj) !== true ? acc : acc + 1 ;
          return acc;
        }, 0);
        if (result < minimumGroupValidations)
        {
          otherGroupMembers.forEach(field => {
            $(field).closest(".input").find('.error').text(`Please complete at least ${minimumGroupValidations} field${minimumGroupValidations > 1 ? "s" : ""} using ${validator($(field).val()).split("Please enter ").pop()}`).show();
          });
          acc = false;
        }
      } else {
      const valid = validator(value, obj);
      if (valid !== true) {
        acc = false;
        errDisplay.text(`${valid}`).show();
      }
    }
    }

    return acc;
  }, true) === true
    ? obj
    : false;
}


export async function validatePostCode(postcode) {
    let postcodeValid = false;
    const resp = await hmsFetch(`/api/postcode/${postcode}`);
    if (resp.fetchResponse == fetchResponses.SUCCESS) {
        const { hasContent, isSuccessful } = await resp.fetchPayload;
        postcodeValid = (hasContent && isSuccessful);
    } else {
        console.error("Invalid Postcode Validation Response");
    }
    return postcodeValid
}


export function validateEmail(email) {        
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    var valid = re.test(email);
    return valid;
}

export function validatePhoneNumber(phoneNumberEl, errorMessage, mobileNumber = false) {
    var phoneNumber = phoneNumberEl.val();
    if (phoneNumber == "") return true;

    var regex = /^\+?(?:[0-9] ?){6,14}[0-9]$/;
    var valid = regex.test(phoneNumber);

    if (valid == false) {
        phoneNumberEl.find("~ .error").show();
        phoneNumberEl.find("~ .error").text(errorMessage);
    }
    return valid;
}

export function  hasNumber(myString) {
    return /\d/.test(myString);
}

export function scrollToFirstError() {
    $('.error').each(function () {
        if ($(this).is(":visible")) {
            let visibleElement = $(this); 
            $("html, body").animate(
                {
                    scrollTop: visibleElement.parent().offset().top,
                },
                {
                    duration: 300,                
                }
            );       
            return false;
        }
    })
   
}