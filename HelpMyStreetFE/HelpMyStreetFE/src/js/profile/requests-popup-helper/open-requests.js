import { showPopup } from '../../shared/popup';

export function showVerifiedAcceptPopup(acceptBtn) {
    let popupMessage =
        `<p>Once you accept a task, you are responsible for working with the person who made the request (and / or the person who needs the help - if it was made on behalf of someone else) to get it done.</p>
            <p>The request will move from this page to your "<a href="/account/acceptedrequests" target="_blank">My Accepted Requests</a>" page - from where you will be able to access more details about it - including contact details by clicking on "View more info".</p>
            <p>You should call the person in need as soon as you can - they may be depending on you!</p>
            <p>Thank you for your help!</p>`
    var jobId = acceptBtn.parentsUntil(".job").parent().attr("id");
    showPopup({
        header: "Accept?",
        htmlContent: popupMessage,
        messageOnFalse: "We couldn't accept this request at the moment, please try again later",
        actionBtnText: "Accept",
        acceptCallbackAsync: async () => {
            let resp = await fetch('/api/requesthelp/accept-request', {
                method: 'post',
                headers: {
                    'content-type': 'application/json'
                },
                body: JSON.stringify({ jobId })
            });
            if (resp.ok) {
                var hasUpdated = await resp.json()
                if (hasUpdated == true) {
                    acceptBtn.text("Accepted");
                    acceptBtn.attr("disabled", "true");
                }
                return hasUpdated;
            } else {
                return false;
            }
        }
    })
}

export function showUnVerifiedAcceptPopup() {
    let popupMessage =
        `<p>To see more details of requests for help and to start accepting them, you need to be <strong>verified.</strong></p>
            <p>In order to protect everyone who uses this site, we need to make sure that everyone is who they say they are.</p>
            <p>Our partner Yoti provides a verification service that compares your face to your photo on an ID document (passport or driving license) and verifies that you’re you. It only takes a few minutes.</p>
            <p>Visit your <a href="/account/profile">My Profile page</a> now to begin the process.</p>`
    showPopup({
        header: "Get Verified",
        htmlContent: popupMessage,
        cssClass: "warning",
        messageOnFalse: "an error occured, please close the popup.",
        actionBtnText: "Get Verified",
        acceptCallbackAsync: () => {
            window.location.href = "/account/profile"
            return true;
        }

    })
}

