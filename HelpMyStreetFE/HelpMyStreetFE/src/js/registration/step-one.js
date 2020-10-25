import clientFirebase from "../firebase";
import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData, validatePrivacyAndTerms } from "../shared/validator";
import { trackEvent } from "../shared/tracking-helper";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";

export function initialiseStepOne() {

  clientFirebase.init(JSON.parse(configuration.firebase));        

  trackEvent("Registration flow", "View Step 1");

  $("#registration_form").on("submit", function (evt) {
    evt.preventDefault();
    const valid = validateFormData($(this), {
      email: (v) => v !== "" || "Please enter an email address",
      password: (v) =>
        RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{10,})").test(v) ||
        "Please use a strong password",
      confirm_password: (v, d) =>
            d.password === v || "Please ensure passwords match",  
      privacy_and_terms: (v) => v === true || "Please tick to indicate that you acknowledge our Privacy Policy and accept our Terms and Conditions."
    });      
    
    trackEvent("Registration flow", "Submit Step 1", valid ? "(Valid)" : "(Invalid)");

    if (valid === false) return;

    const { email, password } = valid;

    var btn = $(this).find("button[type=submit]");

    buttonLoad(btn);

    clientFirebase.auth
      .createUserWithEmailAndPassword(email, password)
      .then(async (credentials) => {
        const token = await credentials.user.getIdToken();

        const fetchAttempt = await hmsFetch("/registration/step-one", {
          method: "post",
          headers: {
            "content-type": "application/json",
          },
          body: JSON.stringify({
            email,
            token,
            referringGroupId,
            source,
          }),
        });
          if (fetchAttempt.fetchResponse == fetchResponses.SUCCESS) {
              window.location.href = "/registration/step-two";
          }
          else {
              setError("email", "An error has occurred, please try again")
          }
      })
      .catch((err) => {
        switch (err.code) {
          case "auth/email-already-in-use":
            setError("email", err.message);
            break;
          case "auth/weak-password":
            setError("password", err.message);
            break;        
          }
          buttonUnload(btn);
      })      

    return false;
  });
}

function setError(formField, message) {
  $(`input[name=${formField}] ~ .error`).text(message).show();
}
