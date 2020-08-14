import {
    getParameterByName,
    updateQueryStringParam,
    removeQueryStringParam
} from "../shared/querystring-helper";
import {
    showUnVerifiedAcceptPopup,
} from "./requests-popup-helper/open-requests"
import {
    buttonLoad,
    buttonUnload
} from "../shared/btn";
import {
    showPopup
} from "../shared/popup";
import {
    getPopupMessaging
} from "./requests-popup-helper/requests-popup-messaging";
import {
    hmsFetch,
    fetchResponses
} from "../shared/hmsFetch";

export function initialiseRequests(isVerified) {
    const job = getParameterByName("j");  

    if (job) {
        var jobEl = $(`#${job}`);
        if (jobEl.length) {
            $("html, body").animate(
                {
                    scrollTop: jobEl.offset().top,
                },
                {
                    duration: 1000,
                    complete: () => {
                        if (isVerified) {
                            $(`#${job} .job__detail`).slideDown();
                            $(`#${job}`).addClass("open highlight");
                            loadJobDetails(jobEl);
                        } else {
                            $(`#${job}`).addClass("highlight");
                        }
                    },
                }
            );
        }
    }

    $('.job-list').on('mouseover', '.job', function () {
        loadJobDetails($(this));
    });

    $('.job-list').on('click', '.job a.open', function (e) {
        e.preventDefault();
        const job = $(this).parentsUntil('.job').parent();
        if (isVerified) {
            updateQueryStringParam('j', $(job).attr('id'));
            job.toggleClass('open');
            job.find('.job__detail').slideToggle();
            loadJobDetails(job);
        } else {
            showUnVerifiedAcceptPopup();
        }
    });

    $('.job-list').on('click', '.job a.close', function (e) {
        e.preventDefault();
        const job = $(this).parentsUntil('.job').parent();
        removeQueryStringParam('j', $(job).attr('id'));
        job.toggleClass('open');
        job.find('.job__detail').slideToggle();
    });

    $('.job-list').on('click', '.job__expander h5', function (e) {
        e.preventDefault();
        $(this).toggleClass('open');
        $(this).next().slideToggle();
    });

    $('.job-list').on('click', '.job button.trigger-status-update-popup', function () {
        showStatusUpdatePopup($(this));
    });

    $('.job-list').on('click', '.accept-request-unverified', function () {
        showUnVerifiedAcceptPopup();
    });

    $('.job-list').on('click', '.undo-request', async function (evt) {
        const job = $(this).parentsUntil(".job").parent();
        const targetState = $(this).data("target-state");
        const targetUser = $(this).data("target-user") ?? "";

        buttonLoad($(this));
        let hasUpdated = await setJobStatus(job, targetState, targetUser);
        if (hasUpdated) {
            $(job).find('.job__status span').html(targetState);
            $(job).find('button').toggle();
        }
        buttonUnload($(this));
    })

    initialiseFilters();
}



export function showStatusUpdatePopup(btn) {
    const job = btn.parentsUntil(".job").parent();
    const targetState = $(btn).data("target-state");
    const targetUser = $(btn).data("target-user") ?? "";

    let popupSettings = getPopupMessaging($(job).data("job-status"), targetState, $(job).data("user-acting-as-admin") === "True");

    popupSettings.acceptCallbackAsync = async () => {
        let success = await setJobStatus(job, targetState, targetUser);

        if (success) {
            $(job).find('.job__status span').html(targetState);
            $(job).find('button').toggle();
            $(job).find('.next-step').toggle();
        }
        return success;
    }

    showPopup(popupSettings);
}




async function setJobStatus(job, newStatus, targetUser) {
    let jobId = job.attr("id");

    var response = await hmsFetch('/api/requesthelp/set-job-status?j=' + jobId + '&s=' + newStatus + '&u=' + targetUser);
    if (response.fetchResponse == fetchResponses.SUCCESS) {
        return response.fetchPayload;
    }
    else {
        return false;
    }
}


async function loadJobDetails(job, forceRefresh) {
    let jobDetail = $(job).find('.job__detail');

    if (!forceRefresh && jobDetail.data('status') !== undefined) {
        return;
    }

    let jobId = $(job).attr("id");
    jobDetail.data('status', 'updating' );
    const response = await hmsFetch('/api/requesthelp/get-job-details?j=' + jobId);
    if (response.fetchResponse == fetchResponses.SUCCESS) {
        jobDetail.html(await response.fetchPayload);
        jobDetail.data('status', { 'updated': new Date() });
    } else {
        jobDetail.removeData('status');
        return false;
    }
}


function initialiseFilters() {
    $('.job-filter-panel').on('click', '.update', async function (e) {
        e.preventDefault();
        const formData = $('.job-filter-panel form').serializeArray();
        let dataToSend = {};

        formData.forEach((d) => {
            if (d.name.indexOf('[]') > 0) {
                const name = d.name.replace('[]', '');
                if (!dataToSend[name]) {
                    dataToSend[name] = [parseInt(d.value)];
                } else {
                    dataToSend[name].push(parseInt(d.value));
                }
            } else {
                dataToSend[d.name] = parseInt(d.value);
            }
        });

        var fetchRequestData = {
            method: 'POST',
            body: JSON.stringify(dataToSend),
            headers: { 'Content-Type': 'application/json' },
        };
        var response = await hmsFetch('/api/requesthelp/get-filtered-jobs', fetchRequestData);
        if (response.fetchResponse == fetchResponses.SUCCESS) {
            $('.job-filter-results-panel .job-list').html(await response.fetchPayload);
        }
        return false;
    });
}