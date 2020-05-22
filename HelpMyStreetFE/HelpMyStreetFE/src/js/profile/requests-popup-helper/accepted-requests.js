import { showPopup } from '../../shared/popup';

export function showCompletePopup(btn) {
    let popupMessage =
        `<p>Thank you!</p>
         <p>This request will be reported as completed and removed from your "My Accepted Tasks" list</p>
         <p>Thanks!</p>`;
    
    var jobId = btn.parentsUntil(".job").parent().attr("id");    
    showPopup({
        header: "Done!",
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
        `<p>Not able to do the task? No problem.</p>
         <p>We will return the task to the "Open Requests" list so that other Volunteers can pick it up.</p>
         <p>Problem with the task? (e.g. contact details don't work) <br />
         Please let us know by emailing <a href="mailto:request@helpmystreet.org">request@helpmystreet.org</a></p>
        <p>Thanks!</p>`;

    let jobId = btn.parentsUntil(".job").parent().attr("id");
    showPopup({
        header: "Release?",
        htmlContent: popupMessage,
        messageOnFalse: "We couldn't release this request at the moment, please try again later",
        actionBtnText: "Confirm",
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
