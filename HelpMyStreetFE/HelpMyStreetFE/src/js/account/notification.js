export const closeNotification = async id => {
  if (!id) throw Error("No notification id to close");

  const options = {
    method: "PUT",
    body: JSON.stringify({ Id: id })
  };
  const promise = fetch("/account/CloseNotification", options);

  //removes all cards w the id
  $(`.notification__card[data-notificationId='${id}']`).fadeOut();
  await promise;
};

export default { closeNotification };
