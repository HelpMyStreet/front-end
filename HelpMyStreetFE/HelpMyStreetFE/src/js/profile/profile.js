import { initialiseVerification } from "./verification";
import { initialiseRequests } from "./requests";
import { initialiseGenericExpanders } from "../ui/generic-expander";
import { initialiseVolunteerList } from "./volunteers";
import { updateAwards } from "../shared/awards";
import { initialiseEditableBiography } from "./editable-biography";


$(document).ready(function () {

    const isVerified = (initObj && initObj.isVerified == "True");
    initialiseVerification(isVerified);
    initialiseRequests();
    initialiseGenericExpanders();
    initialiseVolunteerList();
    updateAwards();
    initialiseEditableBiography();
});
