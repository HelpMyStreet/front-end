import { intialiseRequestStage,  } from "./request-stage";
import { initaliseDetailStage,  } from "./detail-stage";
import { intialiseReviewStage,  } from "./review-stage";
import { trackPageView } from "../shared/tracking-helper";




$(() => {
    const stepType = $('input[name="StepType"]').val();
    switch (stepType) {
        case 'HelpMyStreetFE.Models.RequestHelp.Stages.Request.RequestHelpRequestStageViewModel':
            intialiseRequestStage();
            trackPageView('/request-help/0.request');
            break;
        case 'HelpMyStreetFE.Models.RequestHelp.Stages.Detail.RequestHelpDetailStageViewModel':
            initaliseDetailStage();
            trackPageView('/request-help/1.details');
            break;
        case 'HelpMyStreetFE.Models.RequestHelp.Stages.Review.RequestHelpReviewStageViewModel':
            intialiseReviewStage();
            trackPageView('/request-help/2.review');
            break;
    }
});
