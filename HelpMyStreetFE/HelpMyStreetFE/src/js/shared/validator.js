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