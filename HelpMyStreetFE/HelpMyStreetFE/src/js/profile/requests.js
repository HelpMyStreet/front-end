import {
  getParameterByName,
  updateQueryStringParam,
} from "../shared/querystring-helper";
import {
  showVerifiedAcceptPopup,
  showUnVerifiedAcceptPopup,
} from "./requests-popup-helper/open-requests";

export function initialiseRequests() {
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

      updateQueryStringParam("j", id);
      $(`#${id}`).addClass("open");
      $(`#${id} .job__detail`).slideToggle();
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

  $(".accept-request").click(function (evt) {
    evt.preventDefault();
    if (initObj && initObj.isVerified == "True") {
      showVerifiedAcceptPopup($(this));
    } else {
      showUnVerifiedAcceptPopup();
    }
  });

  $(".job__expander h5").each((_, a) => {
    const el = $(a);

    el.on("click", (e) => {
      e.preventDefault();

      el.toggleClass("open");
      el.next().slideToggle();
    });
  });
}
