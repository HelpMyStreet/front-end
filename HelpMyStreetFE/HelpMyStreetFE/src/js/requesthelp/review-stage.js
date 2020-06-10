import { buttonLoad } from "../shared/btn";

export function intialiseReviewStage() {
    $("form").on("submit", function (evt) {
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

