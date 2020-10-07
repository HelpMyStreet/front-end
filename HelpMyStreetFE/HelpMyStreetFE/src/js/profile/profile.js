import { initialiseVerification } from "./verification";
import { initialiseRequests } from "./requests";

$(document).ready(function () {
    const isVerified = (initObj && initObj.isVerified == "True");
    initialiseVerification(isVerified);
    initialiseRequests(isVerified);
});

