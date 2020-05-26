import {
    getParameterByName,
    updateQueryStringParam
} from "../shared/querystring-helper";
import {
    showVerifiedAcceptPopup,
    showUnVerifiedAcceptPopup,
    SetRequestToInProgress
} from "./requests-popup-helper/open-requests"
import {
    showCompletePopup,
    showReleasePopup
} from "./requests-popup-helper/accepted-requests"
import {
    buttonLoad,
    buttonUnload
} from "../shared/btn";

export function initialiseRequests(isVerified) {
  const job = getParameterByName("j");  
  if (job) {
    $("html, body").animate(
      {
        scrollTop: $(`#${job}`).offset().top,
      },
      {
        duration: 1000,
        complete: () => {
          $(`#${job}`).addClass("open highlight");
          $(`#${job} .job__detail`).slideDown();
        },
      }
    );
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


    $('.accept-request').click(function (evt) {
        evt.preventDefault();        
        if (isVerified) {
            showVerifiedAcceptPopup($(this));
        } else {
            showUnVerifiedAcceptPopup();
        }
    });

    $('.complete-request').click(function (evt) {
        evt.preventDefault();
        showCompletePopup($(this));
    })


  $(".job__expander h5").each((_, a) => {
    const el = $(a);

    el.on("click", (e) => {
      e.preventDefault();

      el.toggleClass("open");
      el.next().slideToggle();
    });
  });

    $('.undo-request').click(async function (evt) {
        evt.preventDefault();  
        let jobId = $(this).parentsUntil(".job").parent().attr("id");
        buttonLoad($(this));
        let hasUpdated = await SetRequestToInProgress(jobId)  
        if (hasUpdated) {
            let type = $(this).attr("data-undo");
            let releaseButton = $(this).prev(".release-request");
            let doneButton = releaseButton.prev(".complete-request");

            switch (type) {
                case "complete":
                    _undoCompleteButtons(releaseButton, doneButton)
                    break;
                case "release":
                    _undoReleaseButtons(releaseButton, doneButton);
                    break;
            }
           
            $(this).hide();            
        }
        buttonUnload($(this));
    })

    $('.release-request').click(function () {
        showReleasePopup($(this))
    })
}


function _undoCompleteButtons(releaseButton, doneButton) {

    releaseButton.show();
    doneButton.text("Completed");
    doneButton.removeClass("actioned");
    doneButton.attr("disabled", false);
}

function _undoReleaseButtons(releaseButton, doneButton) {
    doneButton.show();
    releaseButton.text("Can't Do");
    releaseButton.removeClass("actioned");
    releaseButton.attr("disabled", false);
}