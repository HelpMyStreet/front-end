export function stringifyForm(form) {
  const formData = $(form).serializeArray();
  let formDataAsObject = {};
  formData.forEach((d) => {
    const inputType = $(form).find(`input[name="${d.name}"]`).attr('type');
    let value = d.value;
    if ((inputType == 'radio' || inputType == 'checkbox' || inputType == 'number') && parseFloat(value) != NaN) {
      value = parseFloat(value);
    }

    if (d.name.indexOf('[]') > 0) {
      const name = d.name.replace('[]', '');
      if (!formDataAsObject[name]) {
        formDataAsObject[name] = [value];
      } else {
        formDataAsObject[name].push(value);
      }
    } else {
      formDataAsObject[d.name] = value;
    }
  });
  return JSON.stringify(formDataAsObject);
}