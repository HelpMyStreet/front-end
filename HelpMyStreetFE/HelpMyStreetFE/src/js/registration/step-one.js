import clientFirebase from "../firebase";
import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData, validatePrivacyAndTerms } from "../shared/validator";


export function initialiseStepOne() {

    clientFirebase.init(JSON.parse(configuration.firebase));        

  $("#registration_form").on("submit", function (evt) {
    evt.preventDefault();
    const valid = validateFormData($(this), {
      email: (v) => v !== "" || "Please enter an email address",
      password: (v) =>
        RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{10,})").test(v) ||
        "Please use a strong password",
      confirm_password: (v, d) =>
            d.password === v || "Please ensure passwords match",  
        terms_and_conditions: (v, d) =>
            validatePrivacyAndTerms("privacy_notice", "terms_and_conditions") || ""
          
    });      
    
    if (valid === false) return;

    const { email, password } = valid;

    var btn = $(this).find("button[type=submit]");

    buttonLoad(btn);

    clientFirebase.auth
      .createUserWithEmailAndPassword(email, password)
      .then(async (credentials) => {
        const token = await credentials.user.getIdToken();

        await fetch("/registration/stepone", {
          method: "post",
          headers: {
            "content-type": "application/json",
          },
          body: JSON.stringify({
            email,
            token,
          }),
        });          
          window.location.href = "/registration/steptwo?source=" + source;
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
      })
      .finally(() => buttonUnload(btn));

    return false;
  });
}

function setError(formField, message) {
  $(`input[name=${formField}] ~ .error`).text(message).show();
}
