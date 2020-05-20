import { getParameterByName, updateQueryStringParam } from "../shared/querystring-helper";
import { showPopup } from '../shared/popup';


export function initialiseRequests() {
    showPopup("Accept?", '<p>Once you accept a task, you are responsible for working with the person who made the request (and / or the person who needs the help – if it was made on behalf of someone else) to get it done.</p>'
        + '<p>The request will move from this page to your "My Accepted Requests" page – from where you will be able to access more details about it – including contact details by clicking on “View more info”.</p>'
        + "<p>You should call the person in need as soon as you can – they may be depending on you!</p>"
        + "<p>Thank you for your help!</p>");
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
