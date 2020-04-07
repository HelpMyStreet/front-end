import clientFirebase from "../firebase";
import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData } from "../shared/validator";

export function initialiseStepOne() {
  clientFirebase.init({
    apiKey: "AIzaSyBcXGTnRXhFGq3fb6-ulyo-7qL8P0RIbqA",
    authDomain: "factor50-test.firebaseapp.com",
    databaseURL: "https://factor50-test.firebaseio.com",
    projectId: "factor50-test",
    storageBucket: "factor50-test.appspot.com",
    messagingSenderId: "1075949051901",
    appId: "1:1075949051901:web:1be61ff6f6de11c1934394",
  });

  $("#registration_form").on("submit", function (evt) {
    evt.preventDefault();

    const valid = validateFormData($(this), {
      email: (v) => v !== "" || "Please enter an email address",
      password: (v) =>
        RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{10,})").test(v) ||
        "Please use a strong password",
      confirm_password: (v, d) =>
        d.password === v || "Please ensure passwords match",
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
