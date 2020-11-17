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
                buttonUnload($(this));
            }
        } finally {
            $(this).disabled = false;
        }
    });

    $(".yt-video-placeholder").click(function () {
        var height = $(this).height();
        var width = $(this).width();
        $(this).html('<iframe style="min-width: ' + width + 'px; height: ' + height + 'px" src="https://www.youtube-nocookie.com/embed/BD--FjbDKp8?rel=0&amp;cc_load_policy=1&amp;modestbranding=1;autoplay=1" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen=""></iframe>');
    });
});
