import { initialiseVerification } from "./verification";
import { initialiseRequests } from "./requests";
import { initialiseGenericExpanders } from "../ui/generic-expander";
import { initialiseVolunteerList } from "./volunteers";

$(document).ready(function () {
  const isVerified = (initObj && initObj.isVerified == "True");
  initialiseVerification(isVerified);
  initialiseRequests(isVerified);
  initialiseGenericExpanders();
  initialiseVolunteerList();
});

