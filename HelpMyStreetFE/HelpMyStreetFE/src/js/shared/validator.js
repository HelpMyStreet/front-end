export function validateFormData(form, validation) {
  return form.serializeArray().reduce((acc, cur) => {
    const { name, value } = cur;

    const errDisplay = $(`input[name=${name}] ~ .error`);
    errDisplay && errDisplay.text("").hide();

    const validator = validation[name];

    if (validator) {
      const valid = validator(value);
      if (valid !== true) {
        acc = false;
        errDisplay.text(valid).show();
      }
    }

    return acc;
  }, true);
}
