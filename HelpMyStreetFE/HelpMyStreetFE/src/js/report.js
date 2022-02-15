import { hmsFetch, fetchResponses } from "./shared/hmsFetch.js";

 function label(tooltipItem) {    
    var children = tooltipItem.raw.child;
    var labels = [];     
    if (children!=null && children.length > 0) {
        for (let i = 0; i < children.length; i++) {
            labels.push(children[i].text + " : " + children[i].val);
        }
    }
    else {
        labels.push(tooltipItem.dataset.label + " : " + tooltipItem.raw.y);
    }
    return labels;
};

function title(tooltipItem) {
    if (tooltipItem[0].raw.child != null ) {
        return tooltipItem[0].label + " - " + tooltipItem[0].dataset.label;
    }
    else {
        return tooltipItem[0].label;
    }
};

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
                    Object.assign(reportdata.options.plugins, {
                        tooltip: {
                            callbacks:
                            {
                                label: label,
                                title : title
                            }
                        }
                    });                    
                } else {
                    return [];
                }
                new Chart($(this).find("canvas"),reportdata);
            }
        )
}