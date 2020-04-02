import { initialiseStepOne } from "./registration/step-one";
import { initialiseStepTwo } from "./registration/step-two";

$(() => {
  switch (activeStep) {
    case 1:
      initialiseStepOne();
      break;
    case 2:
      initialiseStepTwo();
      break;
  }
});
