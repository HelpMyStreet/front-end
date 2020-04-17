import clientFirebase from "../firebase";
import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData } from "../shared/validator";


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
      privacy_notice: (v) => v === true || "Please tick to confirm that you acknowledge the Help My Street <a href=' / privacy - policy'>Privacy Notice</a>",
      terms_and_conditions: (v) => v === true || "Please tick to confirm that you agree to the Help My Street <a href=' / terms - conditions'>Terms and Conditions</a>",
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
          console.log("hit");
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
