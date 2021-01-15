import { initialiseVerification } from "./verification";
import { initialiseRequests } from "./requests";
import { initialiseGenericExpanders } from "../ui/generic-expander";
import { initialiseVolunteerList } from "./volunteers";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch";
import { updateAwards } from "../shared/awards";

function updateJobBar() {
    var jbwidth = $(".status-circle").width();
    var dashoffset = aJobs / totalJobs * 3.141 * 0.7 * jbwidth;
    $("#jobBar").animate({ 'stroke-dashoffset': dashoffset });
    $(".status-circle span").html(`${aJobs}/${totalJobs}`);
    if (aJobs == totalJobs) {
        $('#jobBar').animate({"fill-opacity": 1 });
        $('#tick').show();
    } else {
        $('#tick').hide();
    }
}

$(document).ready(function () {
  const isVerified = (initObj && initObj.isVerified == "True");
  initialiseVerification(isVerified);
  initialiseRequests();
  initialiseGenericExpanders();
  initialiseVolunteerList();
  updateAwards();
  updateJobBar();

  $('#addJob').click(function () {
    if (aJobs != totalJobs) {
        aJobs = aJobs + 1;
    }
    updateJobBar();
  });
});
