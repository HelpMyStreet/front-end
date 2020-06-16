import { intialiseRequestStage,  } from "./request-stage";
import { initaliseDetailStage,  } from "./detail-stage";
import { intialiseReviewStage,  } from "./review-stage";
import { trackPageView } from "../shared/tracking-helper";




$(() => {
    switch (stepIndex) {
        case 0:
            intialiseRequestStage();
            trackPageView('/request-help/0.request');
            break;
        case 1:
            initaliseDetailStage();
            trackPageView('/request-help/1.details');
            break;
        case 2:
            intialiseReviewStage();
            trackPageView('/request-help/2.review');
            break;
    }
});
