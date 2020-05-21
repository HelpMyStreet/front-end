import {
    getParameterByName,
    updateQueryStringParam
} from "../shared/querystring-helper";
import {
    showVerifiedAcceptPopup,
    showUnVerifiedAcceptPopup,
    SetRequestToOpen
} from "./requests-popup-helper/open-requests"
import {
    showCompletePopup
} from "./requests-popup-helper/accepted-requests"
import {
    buttonLoad,
    buttonUnload
} from "../shared/btn";

export function initialiseRequests() {
  const job = getParameterByName("j");
  const isVerified = (initObj && initObj.isVerified == "True");
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
        let hasUpdated = await SetRequestToOpen(jobId)  
        if (hasUpdated) {
            let releaseButton = $(this).prev(".release-request");
            let doneButton = releaseButton.prev(".complete-request");
            releaseButton.show();
            doneButton.text("Done");
            doneButton.removeClass("actioned");
            doneButton.attr("disabled", false);
            $(this).hide();            
        }
        buttonUnload($(this));
    })
}
