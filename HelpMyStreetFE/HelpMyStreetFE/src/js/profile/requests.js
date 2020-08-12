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

    $(".job").each(function () {
        $(this).on("mouseover", () => {
            loadJobDetails($(this));
        })
    });

    $(".job a.open").each((_, a) => {
        const el = $(a);
        const id = el.attr("data-id");
        el.on("click", (e) => {
            e.preventDefault();
            if (isVerified) {
                updateQueryStringParam('j', id);
                $(`#${id}`).addClass("open");
                $(`#${id} .job__detail`).slideToggle();
                const job = el.parentsUntil(".job").parent();
                loadJobDetails(job);
            } else {
                showUnVerifiedAcceptPopup();
            }
        });
    });

    $(".job a.close").each((_, a) => {
        const el = $(a);
        const id = el.attr("data-id");
        el.on("click", (e) => {
            e.preventDefault();
            removeQueryStringParam('j', id);
            $(`#${id}`).removeClass("open");
            $(`#${id} .job__detail`).slideToggle();
        });
    });

    $(".job__expander h5").each((_, a) => {
        const el = $(a);

        el.on("click", (e) => {
            e.preventDefault();

            el.toggleClass("open");
            el.next().slideToggle();
        });
    });

    $('.job button.trigger-status-update-popup').click(function () {
        showStatusUpdatePopup($(this));
    });

    $('.accept-request-unverified').click(function () {
        showUnVerifiedAcceptPopup();
    });

    $('.undo-request').click(async function (evt) {
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