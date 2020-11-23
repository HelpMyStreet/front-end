const defaultDivContainer = "awards-component";
const defaultComponentEndpoint = "/account/LoadAwardsComponent";

import { hmsFetch, fetchResponses } from "../shared/hmsFetch";

export async function updateAwards(componentEndpoint, divContainer) {
    componentEndpoint = componentEndpoint ?? defaultComponentEndpoint;
    divContainer = divContainer ?? defaultDivContainer;

    setInterval(async () => {

    var response = await hmsFetch(componentEndpoint);
    if (response.fetchResponse == fetchResponses.SUCCESS) {
        var html = await response.fetchPayload;

        $(`.${divContainer}`).html(html);

    } else if (response.fetchResponse == fetchResponses.UNAUTHORISED) {
        if (window.location.pathname.startsWith('/account/')) {
            // Session expired on logged-in page; redirect to login
            window.location.replace('/account/Login?ReturnUrl=' + encodeURIComponent((window.location.pathname + window.location.search)));
        } else {
            // Session expired on public page; don't redirect, but also don't bother trying to get any more badge refreshes
            clearInterval(awardsInterval);
        }
    } else {
        //something terrible has gone wrong!
    }
    $("#what-is-this").click(() => { $(".tooltiptext").addClass("visible") });
    $("#close-tooltip").click(() => { $(".tooltiptext").removeClass("visible") });
},1000 ) //interval to allow backend to update prior to calling it, probably could be less that 1s
}