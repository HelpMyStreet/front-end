export function getPopupMessaging(currentState, targetState, userActingAsAdmin) {
    let settings = { actionBtnText: "Confirm" };

    if (targetState === "InProgress") {
        if (currentState === "Open") {

            settings.header = "Accept this Request for Help?";
            settings.htmlContent =
                `<p>It will appear on your "My Accepted Requests" page and you’ll be able to view more information about it.</p>
                 <p>Please use the information in the request to fulfil it as soon as possible (this may involve contacting the recipient) – the ball’s in your court, someone may be depending on you.</p>
                 <p>Thank you for helping people in your community to stay safe.</p>`;
            settings.messageOnFalse = "We couldn't accept this request at the moment, please try again later";
            settings.actionBtnText = "Accept";

        } else if (currentState === "Done") {

            settings.header = "Mark as In Progress";
            settings.htmlContent =
                `<p>It will return to the volunteer's "My Accepted Requests" page.</p>`;
            settings.messageOnFalse = "We couldn't update this request at the moment, please try again later";

        }
    } else if (targetState === "Done") {
        if (currentState === "InProgress") {

            settings.header = "Completed?";
            if (userActingAsAdmin) {
                settings.htmlContent =
                    `<p>We’ll mark this request as complete.</p>
                     <p>Thank you for helping people in your community stay safe.</p>`;
            } else {
                settings.htmlContent =
                    `<p>We’ll move this request from your Accepted Requests page.</p>
                     <p>Thank you for helping people in your community stay safe.</p>`;
            }
            settings.messageOnFalse = "We couldn't complete this request at the moment, please try again later";

        }
    } else if (targetState === "Open") {
        if (currentState === "InProgress") {

            if (userActingAsAdmin) {
                settings.header = "Mark as open?";
                settings.htmlContent = 
                    `<p>Request unable to be completed?</p>
                     <p>We can return it to the 'Open Requests' list so that other volunteers can accept it.</p>`;
            } else {
                settings.header = "Can't do?";
                settings.htmlContent = 
                    `<p>Not able to complete the request?</p>
                     <p>If you're not able to complete a request for help, we can return it to the 'Open Requests' list so that other volunteers can accept it.</p>
                     <p>If there's a problem (e.g. incorrect contact details) please let us know:
                     <a href="mailto:requests@helpmystreet.org">requests@helpmystreet.org</a></p>`;
            }
            settings.messageOnFalse = "We couldn't release this request at the moment, please try again later";

        }
    } else if (targetState === "Cancelled") {
        if (currentState === "Open") {

            settings.header = "Cancel?";
            settings.htmlContent = 
                `<p>Request is no longer current?</p>
                 <p>We can mark is as cancelled and remove it from volunteers' Open Requests view.</p>`;

        } else if (currentState === "InProgress") {

            settings.header = "Mark as cancelled?";
            settings.htmlContent = 
                `<p>Request is no longer current?</p>
                 <p>We can mark is as cancelled and remove it from the volunteer's Accepted Requests view.</p>`;

        } else if (currentState === "Done") {

            settings.header = "Mark as cancelled?";
            settings.htmlContent = 
                `<p>Request was not genuine?</p>
                 <p>We can mark it as cancelled.</p>`;

        }
        settings.messageOnFalse = "We couldn't cancel this request at the moment, please try again later";
    }
    return settings;
}