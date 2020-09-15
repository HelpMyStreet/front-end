export function getPopupMessaging(currentState, targetState, userActingAsAdmin, referringGroup) {
  let settings = { actionBtnText: "Confirm" };

  if (targetState === "InProgress") {
    if (currentState === "Open") {

      settings.header = "Accept this Request for Help?";
      settings.htmlContent =
        `<p>It will appear on your “My Accepted Requests” page and you’ll be able to view more information about it.</p>
         <p>Please use the information in the request to fulfil it as soon as possible (this may involve contacting the recipient) – the ball’s in your court, someone may be depending on you.</p>
		     <p>The requester will be notified that their request has been accepted, but won’t be given your contact details.</p>`
      + (referringGroup !== "Generic" ? (`<p>This request was made via a group: <b>` + referringGroup + `</b>. If you accept this request, we’ll need to share your details with the administrator(s) of that group. By clicking “Continue” below, you are acknowledging your acceptance of this.</p>`) : ``)
      + `<p>Thank you for helping people in your community to stay safe.</p>`;
      settings.messageOnFalse = "We couldn’t accept this request at the moment, please try again later";

    } else if (currentState === "Done") {

      settings.header = "Mark as In Progress?";
      settings.htmlContent =
        `<p>Have you become aware that this request has not been completed (though it has been marked as such)?</p>
		     <p>We can set its status back to “In Progress” for you.</p>
		     <p>It will return to the “My Accepted Requests” page of the last volunteer with who it was In Progress.</p>
		     <p>If you need to set it right back to “Open”, you can do that from this page by exiting and returning to it, finding the request again (which will now be marked as “In Progress”) and using the “Mark as Open” button.`;
      settings.messageOnFalse = "We couldn’t update this request at the moment, please try again later";

    }
  } else if (targetState === "Done") {
    if (currentState === "InProgress") {

      settings.header = "Completed?";
      if (userActingAsAdmin) {
        settings.htmlContent =
          `<p>Have you become aware that this task has been completed, but not marked as such?</p>
           <p>We can mark it as Complete on behalf of the volunteer for you.</p>`;
      } else {
        settings.htmlContent =
          `<p>Thank you so much for helping people in your community stay safe!</p>
		       <p>We’ll remove this request from your "My Accepted Requests” page.</p>
           <p>The requester will be notified that the request has been marked as Complete.</p>`;
      }
      settings.messageOnFalse = "We couldn’t complete this request at the moment, please try again later";

    }
  } else if (targetState === "Open") {
    if (currentState === "InProgress") {

      if (userActingAsAdmin) {
        settings.header = "Mark as Open?";
        settings.htmlContent =
          `<p>Have you become aware that the volunteer who accepted this request is unable to complete it?</p>
           <p>We can return it to the “Open Requests” list so that other volunteers can accept it.</p>
		       <p>If there’s a problem with the request (e.g. incorrect contact details) please let us know: 
		       <a href="mailto:support@helpmystreet.org">support@helpmystreet.org</a></p>`;
      } else {
        settings.header = "Can’t do?";
        settings.htmlContent =
          `<p>Not able to complete the request?</p>
           <p>If you’re not able to complete a request for help, we can return it to the “Open Requests” list so that other volunteers can accept it.</p>
           <p>If there’s a problem with the request (e.g. incorrect contact details) please let us know:
           <a href="mailto:requests@helpmystreet.org">requests@helpmystreet.org</a></p>`;
      }
      settings.messageOnFalse = "We couldn’t release this request at the moment, please try again later";

    }
  } else if (targetState === "Cancelled") {
    if (currentState === "Open") {

      settings.header = "Cancel?";
      settings.htmlContent =
        `<p>Is there a problem with this request (e.g. you have become aware that it was entered in error or that the help is no longer needed)?</p>
         <p>We can mark is as Cancelled and remove it from the “Open Requests” list.</p>`;

    } else if (currentState === "InProgress") {

      settings.header = "Cancel?";
      settings.htmlContent =
        `<p>Is there a problem with this request (e.g. you have become aware that it was entered in error or that the help is no longer needed)?</p>
         <p>We can mark is as Cancelled and remove it from the relevant volunteer’s “My Accepted Requests” page.</p>`;

    } else if (currentState === "Done") {

      settings.header = "Mark as cancelled?";
      settings.htmlContent =
        `<p>Is there a problem with this request (e.g. you have become aware that it was entered in error or that the help is no longer needed)?</p>
         <p>We can mark is as Cancelled for your records.</p>`;

    }
    settings.messageOnFalse = "We couldn’t cancel this request at the moment, please try again later";
  }
  return settings;
}