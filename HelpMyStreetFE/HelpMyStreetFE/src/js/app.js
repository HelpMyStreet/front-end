import "../sass/main.scss";
import firebase from "./firebase/index";
import account from "./account";
import notification from "./account/notification";
import "./shared/info-popup";
import "./shared/site-header";

const firebaseConfig = {
  apiKey: "AIzaSyBcXGTnRXhFGq3fb6-ulyo-7qL8P0RIbqA",
  authDomain: "factor50-test.firebaseapp.com",
  databaseURL: "https://factor50-test.firebaseio.com",
  projectId: "factor50-test",
  storageBucket: "factor50-test.appspot.com",
  messagingSenderId: "1075949051901",
  appId: "1:1075949051901:web:1be61ff6f6de11c1934394"
};

firebase.init(firebaseConfig);
window.account = account;

$(function() {
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
      //TODO: Add some sort of loading spinner to indicate stuff is happening
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
