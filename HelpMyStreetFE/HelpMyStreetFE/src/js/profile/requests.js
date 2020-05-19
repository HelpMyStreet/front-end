import { getParameterByName, updateQueryStringParam } from "../shared/querystring-helper";

export function initialiseRequests() {
  const job = getParameterByName("j");

  if (job) {
    $("html, body").animate(
      {
        scrollTop: $(`#job-${job}`).offset().top,
      },
      {
        duration: 1000,
        complete: () => {
          $(`#job-${job}`).addClass("open highlight");
          $(`#job-${job} .job__detail`).slideDown();
        },
      }
    );
  }

  $(".job a.open").each((_, a) => {
    const el = $(a);
    const id = el.attr("data-id");
    el.on("click", (e) => {
      e.preventDefault();

      updateQueryStringParam('j', id);
      $(`#job-${id}`).addClass("open");
      $(`#job-${id} .job__detail`).slideToggle();
    });
  });

  $(".job a.close").each((_, a) => {
    const el = $(a);
    const id = el.attr("data-id");
    el.on("click", (e) => {
      e.preventDefault();

      $(`#job-${id}`).removeClass("open");
      $(`#job-${id} .job__detail`).slideToggle();
    });
  });
}
