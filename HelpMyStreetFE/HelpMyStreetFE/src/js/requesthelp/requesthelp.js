import { intialiseRequestStage,  } from "./request-stage";
import { initaliseDetailStage,  } from "./detail-stage";
import { intialiseReviewStage,  } from "./review-stage";




$(() => {   
    console.log(stepIndex);
    switch (stepIndex) {
        case 0:
            intialiseRequestStage();
            break;
        case 1:
            initaliseDetailStage();
            break;
    }        
});
