import { intialiseRequestStage,  } from "./request-stage";
import { initaliseDetailStage,  } from "./detail-stage";
import { intialiseReviewStage,  } from "./review-stage";
import { trackPageView } from "../shared/tracking-helper";




$(() => {
    switch (stepIndex) {
        case 0:
            intialiseRequestStage();
            break;
        case 1:
            initaliseDetailStage();
            break;
        case 2:
            intialiseReviewStage();
            break;
    }

    trackPageView('/request-help/step' + stepIndex);
});
