import { getParameterByName, updateQueryStringParam } from "../shared/querystring-helper";
import { showPopup } from '../shared/popup';


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


    $('.accept-request').click(function (evt) {
        if ($(this).attr("data-accepted") != "true") {
        let popupMessage = '<p>Once you accept a task, you are responsible for working with the person who made the request (and / or the person who needs the help - if it was made on behalf of someone else) to get it done.</p>'
            + '<p>The request will move from this page to your "<a href="/account/acceptedrequests" target="_blank">My Accepted Requests</a>" page - from where you will be able to access more details about it - including contact details by clicking on "View more info".</p>'
            + "<p>You should call the person in need as soon as you can - they may be depending on you!</p>"
                + "<p>Thank you for your help!</p>"

            showPopup("Accept?", popupMessage, "We couldn't accept this request at the moment, please try again later", async () => {
                var jobId = $(this).parentsUntil(".job").parent().attr("id");                
                let resp = await fetch('/api/requesthelp/accept-request', {
                    method: 'post',
                    headers: {
                        'content-type': 'application/json'
                    },
                    body: JSON.stringify({ jobId })
                });
                if (resp.ok) {
                    var hasUpdated = await resp.json
                    if (hasUpdated == true) {
                        $(this).text("Accepted");
                        $(this).attr("data-accepted", "true");
                    }
                    return hasUpdated;
                } else {
                    return false;
                }
            })
        }
    });
}
