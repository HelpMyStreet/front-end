
import "../sass/main.scss";
import firebase from "./firebase/index";
import account from "./account";
import notification from "./account/notification";
import "./shared/info-popup";
import "./shared/site-header";
import { intialiseCookieConsent } from "./shared/cookie-helper"
import { intialiseForgottonForm } from "./home/forgotton-password"

$(function () {

    if (typeof configuration !== 'undefined') {
        firebase.init(JSON.parse(configuration.firebase));
    }
    window.account = account;

    intialiseCookieConsent();
    intialiseForgottonForm(firebase, account);

  $("#postcode_button").click(function(evt) {
    const postCode = $("#postcode").val();

    if (postCode) {
      $(this).width($(this).width());
      $(this).height($(this).height());

      $("#postcode_notcovered").hide();
      $("#postcode_covered").hide();
      $("#postcode_error").hide();
      $(this)
        .find(".text")
        .hide();
      $(this)
        .find(".loader")
        .show();

      fetch(`/api/postcode/${postCode}`)
        .then(resp => resp.json())
        .then(data => {
          if (data.addresses && data.addresses.length) $("#postcode_covered").show();
          else $("#postcode_notcovered").show();
        })
        .catch(err => {
          $("#postcode_error").show();
        })
        .finally(() => {
          $(this).width(null);
          $(this).height(null);
          $(this)
            .find(".text")
            .show();
          $(this)
            .find(".loader")
            .hide();
        });
    }
  });
  $("#login-submit").click(async () => {
    try {
        $("#login-submit")[0].disabled = true;
        const email = $("#email").val();
        const password = $("#password").val();
        const response = await account.login.login(email, password);
        if (!response.success) {
        $("#login-fail-message").text(response.message);
        $("#login-submit")[0].disabled = false;
      }
    } finally {
        $("#login-submit")[0].disabled = false;
    }
  });
});
