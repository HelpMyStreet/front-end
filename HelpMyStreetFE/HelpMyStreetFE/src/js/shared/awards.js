const defaultDivContainer = "awards-component";
const defaultComponentEndpoint = "/account/get-awards-component";

import { hmsFetch, fetchResponses } from "../shared/hmsFetch";

export async function updateAwards(componentEndpoint, divContainer) {
    componentEndpoint = componentEndpoint ?? defaultComponentEndpoint;
    divContainer = divContainer ?? defaultDivContainer;

    setTimeout(async () => {

        var response = await hmsFetch(componentEndpoint);
        if (response.fetchResponse == fetchResponses.SUCCESS) {
            var html = await response.fetchPayload;

            $(`.${divContainer}`).html(html);
        }

        $("#what-is-this").click(() => { $(".tooltiptext").addClass("visible"); });
        $("#close-tooltip").click(() => { $(".tooltiptext").removeClass("visible"); });
    }, 1000); //interval to allow backend to update prior to calling it, probably could be less that 1s
}