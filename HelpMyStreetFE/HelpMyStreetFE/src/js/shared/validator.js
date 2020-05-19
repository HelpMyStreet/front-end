export function validateFormData(form, validation) {
  const obj = form
    .find(".input")
    .get()
    .reduce((acc, cur) => {
      const inp = $(cur).find("input");
      if (inp[0]) {
        const { name, value, type } = inp[0];

        acc[name] = type === "checkbox" ? inp.is(":checked") : value;
      }

      return acc;
    }, {});

  return Object.entries(obj).reduce((acc, cur) => {
    const [name, value] = cur;

    const errDisplay = $(`input[name=${name}] ~ .error`);
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


export function validateEmail(emailElement, errorMessage) {
    var email = emailElement.val();
    if (email == "") return true;
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    var valid = re.test(email);
    if (valid == false) {
        emailElement.find("~ .error").show();
        emailElement.find("~ .error").text(errorMessage);
    }
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