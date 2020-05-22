import { showPopup } from '../../shared/popup';

export function showVerifiedAcceptPopup(acceptBtn) {
    let popupMessage =
        `<p>It will appear on your "<a href="/account/accepted-requests" target="_blank">My Accepted Requests</a>" page you’ll be able to view more information about it.</p>
            <p>Please call the person who’s requested help as soon as you can - they may be depending on you.</p>
            <p>Thank you for helping people in your community to stay safe</p>`
    var jobId = acceptBtn.parentsUntil(".job").parent().attr("id");
    showPopup({
        header: "Accept the request for help?",
        htmlContent: popupMessage,
        messageOnFalse: "We couldn't accept this request at the moment, please try again later",
        actionBtnText: "Accept",
        acceptCallbackAsync: async () => {
            let hasUpdated = await SetRequestToInProgress(jobId);   
                if (hasUpdated == true) {
                    acceptBtn.text("Accepted");
                    acceptBtn.addClass("actioned");
                    acceptBtn.attr("disabled", "true");
                    let acceptLink = acceptBtn.next(".job__info__footer.actioned")
                    acceptLink.show();
                    acceptLink.next(".job__info__footer").hide();
                }
                return hasUpdated;
            }        
    })
}

export function showUnVerifiedAcceptPopup() {
    let popupMessage =
        `<p><a href="/account/profile">Complete your ID verification</a> today so that you can start helping people!</p>
            <p>You're currently registered as an Unverified volunteer. As soon as you've completed your ID Verification, you'll be a Verified volunteer on HelpMyStreet.org and will be able to help people in your local area.</p>`
    showPopup({
        header: "Start Helping",
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


export async function SetRequestToInProgress(jobId) {
    try {
        let resp = await fetch('/api/requesthelp/accept-request', {
            method: 'post',
            headers: {
                'content-type': 'application/json'
            },
            body: JSON.stringify({ jobId })
        });

        if (resp.ok) {
            return resp.json();
        } else {
            return false;
        }
    } catch (err) {
        console.error(err);
        return false;
    }
}

