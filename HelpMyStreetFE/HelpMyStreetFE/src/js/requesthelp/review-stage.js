import { buttonLoad } from "../shared/btn";

export function intialiseReviewStage() {
    $("form").on("submit", function (evt) {
        buttonLoad($("#btnSubmit"));
    });
}

