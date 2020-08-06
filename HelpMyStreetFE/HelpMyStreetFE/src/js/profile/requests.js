import {
    getParameterByName,
    updateQueryStringParam
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
                        } else {
                            $(`#${job}`).addClass("highlight");
                        }
                    },
                }
            );
        }
  }

  $(".job a.open").each((_, a) => {
    const el = $(a);
    const id = el.attr("data-id");
    el.on("click", (e) => {
      e.preventDefault();
        if (isVerified) {            
            updateQueryStringParam('j', id);
            $(`#${id}`).addClass("open");
            $(`#${id} .job__detail`).slideToggle();
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
        let hasUpdated = await setRequestStatus(job, targetState, targetUser);
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
        let success = await setRequestStatus(job, targetState, targetUser);

        if (success) {
            $(job).find('.job__status span').html(targetState);
            $(job).find('button').toggle();
            $(job).find('.next-step').toggle();
        }
        return success;
    }

    showPopup(popupSettings);
}




async function setRequestStatus(job, newStatus, targetUser) {
    let success = false;
    let jobId = job.attr("id");

    let resp = await fetch('/api/requesthelp/set-request-status?j=' + jobId + '&s=' + newStatus + '&u=' + targetUser);
    if (resp.ok) {
        success = await resp.json()
    }
    return success;
}

