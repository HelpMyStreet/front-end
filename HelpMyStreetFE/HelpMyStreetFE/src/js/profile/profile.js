import { initialiseMyStreet } from "./my-streets";
import { initialiseVerification } from "./verification";
import { initialiseRequests } from "./requests";

$(document).ready(function () {
  initialiseMyStreet();
    initialiseVerification();
    initialiseRequests();
});

