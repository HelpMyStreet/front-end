import { hmsFetch, fetchResponses } from "./shared/hmsFetch.js";

export function InitialiseReports() {
    $(".chart-container").each
        (
            async function () {
                console.log("In function");
                const chart = $(this).data("chart");
                const groupId = $(this).data("group");
                const chartType = $(this).data("type");
                const dateFrom = $(this).data("datefrom");
                const dateTo = $(this).data("dateto");

                let reportdata;
              
                let endpoint = '/api/ReportAPI/getReport?chart=' + chart + '&groupId=' + groupId + '&chartType=' + chartType + '&dateFrom=' + dateFrom + '&dateTo=' + dateTo;
                console.log(endpoint);
                const content = await hmsFetch(endpoint);
                if (content.fetchResponse == fetchResponses.SUCCESS) {
                    let thisPayload = await content.fetchPayload;
                    reportdata = thisPayload.reportData;
                } else {
                    return [];
                }
                new Chart($(this).find("canvas"),reportdata);
            }
        )
}