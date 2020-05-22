import { showPopup } from '../../shared/popup';

export function showCompletePopup(btn) {
    let popupMessage =
        `<p>We’ll move this request from your Accepted Requests page.</p>
         <p>Thank you for helping people in your community stay safe.</p>`;
    
    var jobId = btn.parentsUntil(".job").parent().attr("id");    
    showPopup({
        header: "Completed?",
        htmlContent: popupMessage,
        messageOnFalse: "We couldn't complete this request at the moment, please try again later",
        actionBtnText: "Confirm",
        acceptCallbackAsync: async () => {
            let resp = await fetch('/api/requesthelp/complete-request', {
                method: 'post',
                headers: {
                    'content-type': 'application/json'
                },
                body: JSON.stringify({ jobId })
            });
            
            if (resp.ok) {
                let hasUpdated = await resp.json()                      
                if (hasUpdated == true) {                         
                    _modifyButton(btn, "Complete");
                    let releaseButton = btn.next(".release-request");
                    let undoButton = releaseButton.next(".undo-request");
                    undoButton.attr("data-undo", "complete")
                    releaseButton.hide();
                    undoButton.show();      
                }
                return hasUpdated;
            } else {
                return false;
            }
        }
    })
}


export function showReleasePopup(btn) {
    let popupMessage =
        `<p>Not able to complete the request?</p>
         <p>If you're not able to complete a request for help, we can return it to the 'Open Requests' list so that other volunteers can accept it.</p>
         <p>If there's a problem (e.g. incorrect contact details) please let us know:
         <a href="mailto:requests@helpmystreet.org">requests@helpmystreet.org</a></p>`;

    let jobId = btn.parentsUntil(".job").parent().attr("id");
    showPopup({
        header: "Can't do?",
        htmlContent: popupMessage,
        messageOnFalse: "We couldn't release this request at the moment, please try again later",
        actionBtnText: "Confirm",
        buttonCssClass: "bg-dark-blue",
        acceptCallbackAsync: async () => {
            let resp = await fetch('/api/requesthelp/release-request', {
                method: 'post',
                headers: {
                    'content-type': 'application/json'
                },
                body: JSON.stringify({ jobId })
            });

            if (resp.ok) {
                let hasUpdated = await resp.json()
                if (hasUpdated == true) {
                    _modifyButton(btn, "Released");
            
                    let completeButton = btn.prev(".complete-request");
                    let undoButton = btn.next(".undo-request");
                    undoButton.attr("data-undo", "release")
                    undoButton.show();      
                    completeButton.hide();
                    let moreInfo = btn.parent().next(".job__info__footer")
                    moreInfo.hide();
                }
                return hasUpdated;
            }
        }
    })
}


function _modifyButton(btn, text) {
    btn.text(text);
    btn.addClass("actioned");
    btn.attr("disabled", "true");
}
