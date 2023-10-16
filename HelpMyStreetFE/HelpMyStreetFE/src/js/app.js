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
});
