
export function updateJobBars() {

    $('.job').each(function () {

        var thisId = $(this).attr("id");
        var jbwidth = $(this).find(".status-circle").width();
        var thisJobVariables = jobCompleteness.find(obj => obj.jobId == thisId);

        var dashoffset = thisJobVariables.aJobs / thisJobVariables.totalJobs * 3.141 * 0.7 * jbwidth;
        $(this).find(`.jobBar`).animate({ 'stroke-dashoffset': dashoffset });
        $(this).find(`.status-circle span`).html(`${thisJobVariables.aJobs}/${thisJobVariables.totalJobs}`);

        if (thisJobVariables.aJobs == thisJobVariables.totalJobs) {
            $(this).find(`.jobBar`).animate({ "fill-opacity": 1 });
            $(this).find(`.tick`).show();
        } else {
            $(this).find(`.tick`).hide();
        }
    });
}

export function addJob(jobID) {
    var thisJC = jobCompleteness.find(obj => obj.jobId == jobID);
    if (thisJC.aJobs != thisJC.totalJobs) {
        thisJC.aJobs = thisJC.aJobs + 1;
    }
    updateJobBars();
}

export function initialiseJobBars() {
    $('.job-list').on('click', '.addJob', function (e) {
        e.preventDefault();
        const job = $(this).closest('.job');
        var thisJC = jobCompleteness.find(obj => obj.jobId == job.attr("id"));
        if (thisJC.aJobs != thisJC.totalJobs) {
            thisJC.aJobs = thisJC.aJobs + 1;
        }
        updateJobBars();
    });
    updateJobBars();
}