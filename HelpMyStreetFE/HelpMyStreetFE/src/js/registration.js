import "../sass/main.scss";
import { clientFirebase } from "./firebase";

$(() => {
  $("#user_creation_form").length &&
    $("#user_creation_form").on("submit", function(evt) {
      evt.preventDefault();

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
            el: $(cur)
          };

          return acc;
        }, {});

        const pwValid = validatePassword(password.val, confirm_password.val);
        if (pwValid !== true) setError(confirm_password.el, pwValid);

        var btn = $(this).find("button[type=submit]");

        buttonLoad(btn);

        clientFirebase.auth
            .createUserWithEmailAndPassword(email.val, password.val)
            .then(async credentials => {
                const token = await credentials.user.getIdToken();
                const authResp = await fetch('/api/auth', {
                    method: 'post',
                    headers: {
                        authorization: `Bearer ${token}`,
                        'content-type': 'application/json'
                    },
                    body: JSON.stringify({ token })
                });
            })
            .catch(err => {
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
});

function setError(inputElement, message) {
    inputElement
        .find(".error")
        .text(message)
        .show();
}

function buttonLoad(btn) {
    btn.width(btn.width());
    btn.height(btn.height());
    btn.find(".text").hide();
    btn.find(".loader").show();
}

function buttonUnload(btn) {
    btn.width(null);
    btn.height(null);
    btn.find(".text").show();
    btn.find(".loader").hide();
}

function validatePassword(pass, check) {
    return pass === check || "Please ensure the passwords match";
}
