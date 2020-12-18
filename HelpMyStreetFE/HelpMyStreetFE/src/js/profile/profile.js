import { initialiseVerification } from "./verification";
import { initialiseRequests } from "./requests";
import { initialiseGenericExpanders } from "../ui/generic-expander";
import { initialiseVolunteerList } from "./volunteers";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch";
import { updateAwards } from "../shared/awards";

$(document).ready(function () {
  const isVerified = (initObj && initObj.isVerified == "True");
  initialiseVerification(isVerified);
  initialiseRequests();
  initialiseGenericExpanders();
  initialiseVolunteerList();
  updateAwards();
});
