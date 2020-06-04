export function validateFormData(form, validation) {
  const obj = form
    .find(".input")
    .get()
    .reduce((acc, cur) => {
      const inp = $(cur).find("input");
      if (inp[0]) {
          const { name, value, type } = inp[0];
          acc[name] = (type === "checkbox" && type !== "radio") ? inp.is(":checked") : value;
          acc[name] = (type === "radio" && !inp.is(":checked") && type !== "checkbox") ? undefined : value;
      }

      return acc;
    }, {});
    
  return Object.entries(obj).reduce((acc, cur) => {
    const [name, value] = cur;

    const errDisplay = $(`input[name="${name}"] ~ .error`);
      
    errDisplay && errDisplay.text("").hide();

      const validator = validation[name];      
      if (validator) {          
        const valid = validator(value, obj);
      if (valid !== true) {
        acc = false;
        errDisplay.text(valid).show();
      }
    }

    return acc;
  }, true) === true
    ? obj
    : false;
}


export async function validatePostCode(postcode) {
    let postcodeValid = false;
    const resp = await fetch(`/api/postcode/${postcode}`);
    if (resp.ok) {
        const { hasContent, isSuccessful } = await resp.json();  
        postcodeValid = (hasContent && isSuccessful);        
    }
    return postcodeValid
}


export function validateEmail(email) {        
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    var valid = re.test(email);
    return valid;
}

export function validatePhoneNumber(phoneNumberEl, errorMessage) {
    var phoneNumber = phoneNumberEl.val();
    if (phoneNumber == "") return true;
    var valid = ((phoneNumber.replace(" ", "").length === 10 || phoneNumber.replace(" ", "").length === 11) && phoneNumber[0] === "0");
    if (valid == false) {
        phoneNumberEl.find("~ .error").show();
        phoneNumberEl.find("~ .error").text(errorMessage);
    }
    return valid;
}

export function  hasNumber(myString) {
    return /\d/.test(myString);
}

export function validatePrivacyAndTerms(privacyName, termsName) {
    // requires checking of two or more inputs at the same time, so cant use the validateFormData.
    $('.termsprivacy').hide();
    let privacy = $("input[name='" + privacyName + "']").is(":checked");
    let terms = $("input[name='" + termsName + "']").is(":checked");
    var errorText = "";
    privacy == false && terms == false ? errorText = "Please tick to indicate that you acknowledge our Privacy Policy and accept our Terms and Conditions." : "";
    privacy == true && terms == false ? errorText = "Please tick to confirm that you agree to the Help My Street <a href='/terms-conditions'>Terms and Conditions</a>" : "";
    privacy == false && terms == true ? errorText = "Please tick to confirm that you acknowledge the Help My Street <a href='/privacy-policy'>Privacy Notice</a>" : "";

    $('.termsprivacy').show();
    $('.termsprivacy').html(errorText);

    if (errorText !== "") {
        return false;
    }
    return true;
}
