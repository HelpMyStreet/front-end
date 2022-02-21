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
                    console.log(endpoint);
                    const content = await hmsFetch(endpoint);
                    if (content.fetchResponse == fetchResponses.SUCCESS) {
                        let thisPayload = await content.fetchPayload;
                        $(this).find(".datatable").replaceWith('<div class="datatable">' + thisPayload + '</div>');                        
                        
                    } else {
                        return [];
                    }                  
                }
                else {
                    let endpoint = '/api/ReportAPI/getReport?chart=' + chart + '&groupId=' + groupId + '&chartType=' + chartType + '&dateFrom=' + dateFrom + '&dateTo=' + dateTo;
                    console.log(endpoint);
                    const content = await hmsFetch(endpoint);
                    if (content.fetchResponse == fetchResponses.SUCCESS) {
                        let thisPayload = await content.fetchPayload;
                        reportdata = thisPayload.reportData;
                    } else {
                        return [];
                    }
                    new Chart($(this).find("canvas"), reportdata);
                }
            }
        )
}