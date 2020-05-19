import { processYoti } from "./profile/yoti";
import { getParameterByName } from "./shared/querystring-helper"

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
