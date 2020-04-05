import clientFirebase from "../firebase";
import { buttonLoad, buttonUnload } from "../shared/btn";

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

  $("#registration_form").on("submit", function () {
    const { email, password, confirm_password } = $(this)
      .find(".input")
      .get()
      .reduce((acc, cur) => {
        const inp = $(cur).find("input");
        const { name, value, type } = inp[0];

        const errDisplay = $(cur).find(".error");
        errDisplay && errDisplay.text("").hide();

        acc[name] = {
          val: type === "checkbox" ? value === "on" : value,
          el: $(cur),
        };

        return acc;
      }, {});

    const pwValid = validatePassword(password.val, confirm_password.val);
    if (pwValid !== true) setError(confirm_password.el, pwValid);

    var btn = $(this).find("button[type=submit]");

    buttonLoad(btn);

    clientFirebase.auth
      .createUserWithEmailAndPassword(email.val, password.val)
      .then(async (credentials) => {
        const token = await credentials.user.getIdToken();

        await fetch("/registration/stepone", {
          method: "post",
          headers: {
            "content-type": "application/json",
          },
          body: JSON.stringify({
            email: email.val,
            token,
          }),
        });

        window.location.href = "/registration/steptwo";
      })
      .catch((err) => {
        switch (err.code) {
          case "auth/email-already-in-use":
            setError(email.el, err.message);
            break;
          case "auth/weak-password":
            setError(password.el, err.message);
            break;
        }
      })
      .finally(() => buttonUnload(btn));

    return false;
  });
}

function setError(inputElement, message) {
  inputElement.find(".error").text(message).show();
}

function validatePassword(pass, check) {
  return pass === check || "Please ensure the passwords match";
}
