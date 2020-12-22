import { buttonLoad, buttonUnload } from "../shared/btn";
import { showServerSidePopup } from "../shared/popup";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch";
import { showFeedbackPopup } from "../feedback/feedback-capture";
import { updateAwards } from "../shared/awards";

export function initialiseRequests() {

    $('.job-list').on('mouseover', '.job', function () {
        loadJobDetails($(this));
    });

    $('.job-list').on('click', '.job a.open', function (e) {
        e.preventDefault();
        const job = $(this).closest('.job');
        job.toggleClass('open');
        job.find('.job__detail').slideToggle();
        loadJobDetails(job);
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
        const job = $(this).closest('.job');
        let jobId = job.attr("id");
        let response = await hmsFetch(`/account/emailJobDetails?j=${jobId}`, null, { timeOutRetry: 0});
        var outcome = 'successful';
        if (response.fetchResponse == fetchResponses.SUCCESS) {
            outcome = 'failed';
        }
        $('#email-notification').addClass(outcome);
        $('#email-notification').addClass("visible");
        $('#email-notification').html(`<p>Email sending ${outcome}<p>`);
        setTimeout(() => { $('#email-notification').removeClass("visible"); $('#email-notification').removeClass(outcome); }, 3000);
    });

    loadFeedbackComponents();
}


export function showStatusUpdatePopup(btn) {
    const job = btn.closest(".job");
    const targetState = $(btn).data("target-state");
    const targetUser = $(btn).data("target-user") ?? "self";

    let jobId = job.attr("id");
    const role = $(job).data("role");

    let popupSource = `/api/request-help/get-status-change-popup?j=${jobId}&s=${targetState}`;

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


async function setJobStatus(job, newStatus, targetUser) {
    const jobId = job.attr("id");
    const role = $(job).data("role");

    return await hmsFetch('/api/request-help/set-job-status?j=' + jobId + '&s=' + newStatus + '&u=' + targetUser + '&r=' + role);
}


async function loadJobDetails(job, forceRefresh) {
  const jobDetail = $(job).find('.job__detail');

  if (!forceRefresh && jobDetail.data('status') !== undefined) {
    return;
  }

  const jobId = $(job).attr("id");
  const jobSet = $(job).closest('.job-filter-results-panel').data('jobset');
  jobDetail.data('status', 'updating');
  const response = await hmsFetch('/api/request-help/get-job-details?j=' + jobId + '&js=' + jobSet);
  if (response.fetchResponse == fetchResponses.SUCCESS) {
    jobDetail.html(await response.fetchPayload);
    jobDetail.data('status', { 'updated': new Date() });
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
        job.find('.feedback-container').html(await response.fetchPayload);
    } else {
        return false;
    }
}