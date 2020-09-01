import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";

export const closeNotification = async id => {
  if (!id) throw Error("No notification id to close");

  const options = {
    method: "PUT",
    body: JSON.stringify({ Id: id })
  };
    const promise = await hmsFetch("/account/CloseNotification", options);


  //removes all cards w the id
    $(`.notification__card[data-notificationId='${id}']`).fadeOut();
    if (promise.fetchResponse != fetchResponses.SUCCESS) {
        throw Error("Notification not closed");
    }
};

export default { closeNotification };
