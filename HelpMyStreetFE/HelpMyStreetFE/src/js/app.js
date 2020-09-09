//IE POLLYFILS for fetch
import 'core-js/';
//---------------------
import "../sass/main.scss";
import firebase from "./firebase/index";
import account from "./account";
import "./shared/info-popup";
import "./shared/site-header";
import "./ui/filters";
import { intialiseCookieConsent } from "./shared/cookie-helper"
import { intialiseForgottonForm } from "./home/forgotton-password"
import { buttonLoad, buttonUnload } from "./shared/btn";

$(function () {
    $('.no-fouc').removeClass('no-fouc');
    if (typeof configuration !== 'undefined') {
        firebase.init(JSON.parse(configuration.firebase));
    }
 
    window.account = account;

    intialiseCookieConsent();
    intialiseForgottonForm(firebase, account);

    $("#login-submit").click(async () => {   
        buttonLoad($(this));   
    try {
        $(this).disabled = true;
        const email = $("#email").val();
        const password = $("#password").val();
        const response = await account.login.login(email, password);        
        if (!response.success) {
        $("#login-fail-message").text(response.message);
        $(this).disabled = false;
      }
    } finally {
        buttonUnload($(this));
        $(this).disabled = false;
        }
    });
});
