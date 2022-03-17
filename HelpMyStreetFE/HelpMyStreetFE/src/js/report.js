import { hmsFetch, fetchResponses } from "./shared/hmsFetch.js";

export function InitialiseReports() {
    $(".chart-container").each
        (
            async function () {
                const chart = $(this).data("chart");
                const groupId = $(this).data("group");
                const chartType = $(this).data("type");
                const dateFrom = $(this).data("datefrom");
                const dateTo = $(this).data("dateto");

                let reportdata;

                if (chartType == "DataTable") {
                    let endpoint = '/api/ReportAPI/getDataTable?chart=' + chart + '&groupId=' + groupId + '&dateFrom=' + dateFrom + '&dateTo=' + dateTo;
                    const content = await hmsFetch(endpoint);
                    if (content.fetchResponse == fetchResponses.SUCCESS) {
                        let thisPayload = await content.fetchPayload;
                        $(this).addClass("datatable")
                            .html(thisPayload);
                        
                    } else {
                        return [];
                    }                  
                }
                else {
                    let endpoint = '/api/ReportAPI/getReport?chart=' + chart + '&groupId=' + groupId + '&chartType=' + chartType + '&dateFrom=' + dateFrom + '&dateTo=' + dateTo;
                    const content = await hmsFetch(endpoint);
                    if (content.fetchResponse == fetchResponses.SUCCESS) {
                        let thisPayload = await content.fetchPayload;
                        reportdata = thisPayload.reportData;
                    } else {
                        return [];
                    }

                    var ctx = document.createElement("canvas");
                    $(this).append(ctx);
                    return new Chart(ctx, reportdata);
                }
            }
        )
}