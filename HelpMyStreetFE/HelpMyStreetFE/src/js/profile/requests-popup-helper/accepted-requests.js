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
                    btn.text("Complete");
                    btn.addClass("actioned");
                    btn.attr("disabled", "true");
                    let releaseButton = btn.next(".release-request");
                    let undoButton = releaseButton.next(".undo-request");
                    releaseButton.hide();
                    undoButton.show();
                    btn.parent().next(".job__info__footer").hide();    
                }
                return hasUpdated;
            } else {
                return false;
            }
        }
    })
}


