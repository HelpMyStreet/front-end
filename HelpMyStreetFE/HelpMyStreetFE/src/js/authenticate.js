import { processYoti } from "./profile/yoti";

var getParameterByName = function (name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

$(document).ready(function () {
    var token = getParameterByName("token");
    var userId = getParameterByName("u");
    var mobile = getParameterByName("mobile");
    processYoti(token, userId, mobile).then(function () {
        console.info("Processed")
    }).catch(function (ex) {
        console.error(ex)
    });
    
});
