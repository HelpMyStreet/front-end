export function stringifyForm(form) {
  const formData = $(form).serializeArray();
  let dataToSend = {};
  formData.forEach((d) => {
    if ($(form).find(`input[name="${d.name}"]`).attr('type') == 'number') {
      dataToSend[d.name] = parseFloat(d.value);
    } else {
      dataToSend[d.name] = d.value;
    }
  });
  return JSON.stringify(dataToSend);
}