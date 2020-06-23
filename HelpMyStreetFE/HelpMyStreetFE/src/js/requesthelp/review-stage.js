import { buttonLoad } from "../shared/btn";
import { trackEvent } from "../shared/tracking-helper";

export function intialiseReviewStage() {
    trackEvent("Request form", "View 2.review", "", 0);

    $("form").on("submit", function (evt) {
        trackEvent("Request form", "Submit 2.review", "", 0);
        buttonLoad($("#btnSubmit"));
    });
    
    if ($("#recipient").length > 0) {
        var recipient = $('#recipient').height();
        var requestor = $('#requestor').height();
        if (recipient > requestor)
            $('#requestor').css('height', (recipient + 30) + "px");
        else
            $('#recipient').css('height', (requestor + 30) + "px");
    }
}

