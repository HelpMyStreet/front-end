import { showPopup } from '../../shared/popup';

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

