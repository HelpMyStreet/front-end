import { initialiseStepOne } from "./registration/step-one";
import { initialiseStepTwo } from "./registration/step-two";
import { initialiseStepThree } from "./registration/step-three";
import { initialiseStepFour } from "./registration/step-four";

$(() => {
  switch (activeStep) {
    case 1:
      initialiseStepOne();
      break;
    case 2:
      initialiseStepTwo();
      break;
    case 3:
      initialiseStepThree();
      break;
    case 4:
      initialiseStepFour();
      break;
  }
});
