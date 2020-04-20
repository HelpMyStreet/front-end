import clientFirebase from "../firebase";
import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData } from "../shared/validator";

function validatePrivacyAndTerms() {
    // requires checking of two or more inputs at the same time, so cant use the validateFormData.
    $('.termsprivacy').hide();
    let privacy = $("input[name='privacy_notice']").is(":checked");
    let terms = $("input[name='terms_and_conditions']").is(":checked");
    var errorText = "";
    privacy == false && terms == false ? errorText = "Please tick to indicate that you acknowledge our Privacy Policy and accept our Terms and Conditions." : "";
    privacy == true && terms == false ? errorText = "Please tick to confirm that you agree to the Help My Street <a href='/terms-conditions'>Terms and Conditions</a>" : "";
    privacy == false && terms == true ? errorText = "Please tick to confirm that you acknowledge the Help My Street <a href='/privacy-policy'>Privacy Notice</a>" : "";

    $('.termsprivacy').show();
    $('.termsprivacy').html(errorText);
    
    if (errorText !== "") {        
        return false;
    }
    return true;
}

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
            validatePrivacyAndTerms() || ""
          
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
        window.location.href = "/registration/steptwo";
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
