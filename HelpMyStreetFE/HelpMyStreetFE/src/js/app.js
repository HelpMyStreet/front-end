
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
      $("#postcode_invalid").hide();
      //$("#request_help").hide();        
      $(".postcode__info").hide();
      $('#postcode_button').removeClass('postcode_button_clicked')

      $(this)
        .find(".text")
        .hide();
      $(this)
        .find(".loader")
        .show();

        fetch(`/api/postcode/checkCoverage/${postCode}`)
        .then(resp => resp.json())
            .then(data => {
                $('#postcode_button').addClass('postcode_button_clicked')
                console.log(data);
                var postCodeValid = (data.postCodeResponse.isSuccessful && data.postCodeResponse.hasContent);                
                if (postCodeValid == false) {
                    $(".postcode__info, #postcode_invalid").show();
                } else {
                    if (data.volunteerCount == 0 && data.championCount == 0)
                        $(".postcode__info, #postcode_notcovered").show();
                    else if (data.volunteerCount > 0 || data.championCount > 0) {
                        $(".postcode__info, #postcode_covered").show();
                        if (data.volunteerCount > 0 && data.championCount > 0) {
                            $(".postcode__info").show();
                           // $("#request_help").hide();  //phase 1.1
                        }
                    }                        
                }              
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
