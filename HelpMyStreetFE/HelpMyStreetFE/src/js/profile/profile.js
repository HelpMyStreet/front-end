import { initialiseMyStreet } from "./my-streets";
import { initialiseVerification } from "./verification";
import { initialiseRequests } from "./requests";

$(document).ready(function () {
    const isVerified = (initObj && initObj.isVerified == "True");
    initialiseMyStreet();
    initialiseVerification(isVerified);
    initialiseRequests(isVerified);
});

