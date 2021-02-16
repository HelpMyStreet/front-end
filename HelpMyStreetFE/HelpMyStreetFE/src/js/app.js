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
    $(".login-form").submit(function (event) {
        event.preventDefault();
        buttonLoad($(this).find("button"));
        const email = $("#email").val();
        const password = $("#password").val();
        account.login.login(email, password);
    });

    $("#sign-up").click(function(event) {
        event.preventDefault();
        const email = $("#email").val();
        let destination = $(this).attr('href');
        if (email) {
            destination = `${destination}?email=${email}`;
        }
        window.location.href = destination;
    });

});
