import { buttonLoad, buttonUnload } from "../shared/btn";
import { showServerSidePopup } from "../shared/popup";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch";
import { showFeedbackPopup } from "../feedback/feedback-capture";
import { updateAwards } from "../shared/awards";
import { enableMaps, drawMap } from "../shared/maps";

let mapsAreGo;

export async function initialiseRequests() {

    $('.job-list').on('mouseover', '.job', function () {
        loadJobDetails($(this));
    });

    $('.job-list').on('click', '.job a.open', function (e) {
        e.preventDefault();
        const job = $(this).closest('.job');
        job.toggleClass('open');
        job.find('.job__detail').slideToggle();
        loadJobDetails(job).then(async () => {
            var canMap = await mapsAreGo;
            if (canMap){
                createMap(job);
            }
        });
    });

    $('.job-list').on('click', '.job a.close', function (e) {
        e.preventDefault();
        const job = $(this).closest('.job');
        job.toggleClass('open');
        job.find('.job__detail').slideToggle();
    });

    $('.job-list').on('click', '.job__expander h4', function (e) {
        e.preventDefault();
        $(this).toggleClass('open');
        $(this).next().slideToggle();
        
    });

    $('.job-list').on('click', '.job button.trigger-status-update-popup', function () {
        showStatusUpdatePopup($(this));
    });

    $('.job-list').on('click', '.job button.trigger-feedback-popup', function () {
        const job = $(this).closest('.job');
        let jobId = job.attr("id");
        const role = $(job).data("role");
        showFeedbackPopup(jobId, role, function () { loadFeedbackComponent(job); });
    });

    $('.job-list').on('click', '.undo-request', async function (evt) {
        const job = $(this).closest(".job");
        const targetState = $(this).data("target-state");
        const targetUser = $(this).data("target-user") ?? "";

        buttonLoad($(this));
        let response = await setJobStatus(job, targetState, targetUser);
        if (response.fetchResponse == fetchResponses.SUCCESS) {
            updateAwards();
            $(job).find('.job__status__new').html('');
            $(job).find('.toggle-on-status-change').toggle();
            $(job).find('button').toggle();
        }
        buttonUnload($(this));
    });

    const highlightedJob = $('.job-list .job.highlight');
    if (highlightedJob.length) {
        $(document.scrollingElement || document.documentElement).animate(
            {
                scrollTop: highlightedJob.offset().top - 20
            },
            {
                duration: 1000,
                complete: () => {
                    highlightedJob.find('.open').click();
                }
            }
        );
    }

    $('.job-list').on('click', '.email-details', async function (evt) {
        evt.preventDefault();

        const job = $(this).closest('.job');

        $(job).find('.email-details').addClass("sending");
        $(job).find('.email-details img').attr("src", "/img/loading_spinner.svg");
        $(job).find('.email-details span').html("Sending...");

        let jobId = job.attr("id");
        let response = await hmsFetch(`/account/email-job-details?j=${jobId}`, null, { timeOutRetry: 0});

        $(job).find('.email-details').removeClass("sending");

        if (response.fetchResponse == fetchResponses.SUCCESS) {
            $(job).find('.email-details').addClass("successful");
            $(job).find('.email-details img').attr("src", "/img/icons/green-tick.svg");
            $(job).find('.email-details span').html("Queued");
            $(job).find('.email-details').attr("title", "Email successfully queued");
            setTimeout(() => {
                $(job).find('.email-details').removeClass("successful");
                $(job).find('.email-details img').attr("src", "/img/icons/email.svg");
                $(job).find('.email-details span').html("Email");
            }, 5000);
        } else {
            $(job).find('.email-details').addClass("failed");
            $(job).find('.email-details img').attr("src", "/img/icons/status/cancelled.svg");
            $(job).find('.email-details span').html("Failed");
            $(job).find('.email-details').attr("title", "Email not sent");
            setTimeout(() => {
                $(job).find('.email-details').removeClass("failed");
                $(job).find('.email-details img').attr("src", "/img/icons/email.svg");
                $(job).find('.email-details span').html("Retry");
            }, 5000);
        }


    });

    mapsAreGo = await enableMaps();
    loadFeedbackComponents();
    
}

async function createMap(job){
    if ($(`#map-${job.attr("id")}`).length != 0){
        var linkResponse = await hmsFetch('/account/get-directions-link?j=' + job.attr("id"))
        var marker;

        if (linkResponse.fetchResponse == fetchResponses.SUCCESS){
            var link = await linkResponse.fetchPayload;
            marker = {
                clickable: true,
                position: {lat: Number(job.find(".location-map").data("lat")), lng: Number(job.find(".location-map").data("lng"))},
                title: "Click for directions",
                origin: new google.maps.Point(0, 35),
                icon: {
                    url: "/img/logos/markers/vaccination-marker.svg",
                    scaledSize: new google.maps.Size(50, 70),
                },
                
                clickListener: () => {window.open(link , "_blank")}
            }
        } else {
            marker = true;
        }
        
        var thisMap = {
                    displayVolunteers: false,
                    displayGroups: false,
                    allowNavigation: false,
                    allowSearch: false,
                    consoleCoordinates: false,
                    initialLat: Number(job.find(".location-map").data("lat")) + 0.001, //Otherwise the map doesn't centre around the pin (because the pin is a tall rectangle and the map is a wide rectangle)
                    initialLng: Number(job.find(".location-map").data("lng")),
                    initialZoom: 14,
                    divID: "map-" + job.attr("id"),
                    singlePin: marker
                };

        drawMap(thisMap);
    }
}

export function showStatusUpdatePopup(btn) {
    const job = btn.closest(".job");
    const targetState = $(btn).data("target-state");
    const targetUser = $(btn).data("target-user") ?? "self";

    const jobId = job.attr("id");
    const requestId = $(job).attr("request-id");
    const role = $(job).data("role");

    let popupSource = `/api/request-help/get-status-change-popup?j=${jobId}&rq=${requestId}&s=${targetState}`;

    let popupSettings = {
        acceptCallbackAsync: async () => {
            let response = await setJobStatus(job, targetState, targetUser);
            if (response.fetchResponse == fetchResponses.SUCCESS) {
                const payload = await response.fetchPayload;
                $(job).find('.job__status__new').html(payload.newStatus);
                $(job).find('button').toggle();
                $(job).find('.toggle-on-status-change').toggle();
                if (payload.requestFeedback === true) {
                    showFeedbackPopup(jobId, role);
                }
        updateAwards();
                return true;
            } else {
                switch (response.fetchResponse) {
                    case fetchResponses.UNAUTHORISED:
                    case fetchResponses.BAD_REQUEST:
                        popupSettings.messageOnFalse = "Sorry, we couldn't update that request. Another user may have updated the same request; please refresh your browser window.";
                        break;
                    case fetchResponses.SERVER_ERROR:
                    case fetchResponses.SERVER_NOT_FOUND:
                    case fetchResponses.TIMEOUT:
                    case fetchResponses.BAD_FETCH:
                        popupSettings.messageOnFalse = "Sorry, we couldn't update that request. Please try again using the button below.";
                }
                return false;
            }
        }
    };

    showServerSidePopup(popupSource, popupSettings);
}


async function setJobStatus(job, targetState, targetUser) {
    const jobId = job.attr("id");
    const requestId = $(job).attr("request-id");
    const role = $(job).data("role");

    return await hmsFetch(`/api/request-help/set-job-status?j=${jobId}&rq=${requestId}&s=${targetState}&u=${targetUser}&r=${role}`);
}


async function loadJobDetails(job, forceRefresh) {
    const jobDetail = $(job).find('.job__detail');

    if (!forceRefresh && jobDetail.data('status') !== undefined) {
        return;
    }

    const jobId = $(job).attr("id");
    const requestId = $(job).attr("request-id");
    const jobSet = $(job).closest('.job-filter-results-panel').data('jobset');
    jobDetail.data('status', 'updating');
    const response = await hmsFetch('/api/request-help/get-job-details?j=' + jobId + '&rq=' + requestId + '&js=' + jobSet);
    if (response.fetchResponse == fetchResponses.SUCCESS) {
        jobDetail.html(await response.fetchPayload);
        jobDetail.data('status', { 'updated': new Date() });
        if (jobId == "") { // Request
            $(jobDetail).find('.job').each(function () {
                if ($(this).find('.feedback-container').length > 0)
                    loadFeedbackComponent($(this));
            });
        }
    } else {
        jobDetail.removeData('status');
        return false;
    }
}

export async function loadFeedbackComponents() {
    $('.job-list .job').each(function () {
        if ($(this).find('.feedback-container').length > 0)
        loadFeedbackComponent($(this));
    });
}

async function loadFeedbackComponent(job) {
    const jobId = job.attr("id");
    const role = $(job).data("role");

    const response = await hmsFetch('/api/request-help/get-feedback-component?j=' + jobId + '&r=' + role);
    if (response.fetchResponse == fetchResponses.SUCCESS) {
        const feedback = await response.fetchPayload;
        if (feedback) {
            job.find('.feedback-container').html(feedback);
        }
    } else {
        return false;
    }
}