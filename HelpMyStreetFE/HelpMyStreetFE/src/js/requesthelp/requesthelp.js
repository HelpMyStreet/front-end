import { intialiseRequestStage,  } from "./request-stage";
import { initaliseDetailStage,  } from "./detail-stage";
import { intialiseReviewStage,  } from "./review-stage";




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
});


function validateQuestion(el) {
    console.log(el);
}
