import { hmsFetch, fetchResponses } from "../shared/hmsFetch";
import { buttonUnload, buttonLoad } from '../shared/btn';

const toggleButtons = document.querySelectorAll(".btn__toggle-show");

toggleButtons.forEach((btn) => {
  btn.addEventListener("click", (e) => {
    e.preventDefault();
    const target = document.getElementById(
      btn.getAttribute("data-target-item")
    );

    if (!target.classList.contains("filter--show")) {
      target.classList.remove("applied");
    }

    if (target) {
      target.classList.toggle("filter--show");
    }

    const g = document.getElementById(btn.getAttribute("data-target-group"));
    g.querySelectorAll(".btn--apply-filter").forEach((b) => {
      b.classList.remove("applied");
      b.classList.remove("disabled");
    });
  });
});

// Apply filter button
const applyButtons = document.querySelectorAll(".btn--apply-filter");
applyButtons.forEach((b) => {
  b.addEventListener("click", async (e) => {
    e.preventDefault();
    buttonLoad($(b));
    b.classList.remove("applied");

    const g = document.getElementById(b.getAttribute("data-target-group"));

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
      g.querySelectorAll('.group--filters-jobs').forEach((t) => {
        t.classList.remove("filter--show");
      });
      b.classList.add("applied");
    }
    buttonUnload($(b));
    return false;
  });
});



  $('.job-filter-panel').on('click', '.show-more-jobs', function (e) {
    e.preventDefault();
    const resultsToShowInput = $(this).closest('.job-filter-panel').find('form input[name="ResultsToShow"]');
    const resultsToShowIncrementInput = $(this).closest('.job-filter-panel').find('form input[name="ResultsToShowIncrement"]');
    resultsToShowInput.val(parseInt(resultsToShowInput.val()) + parseInt(resultsToShowIncrementInput.val()));
    $(this).closest('.job-filter-panel').find('.update').click();
  });

  $('.job-filter-panel').on('click', '.show-all-jobs', function (e) {
    e.preventDefault();
    const resultsToShowInput = $(this).closest('.job-filter-panel').find('form input[name="ResultsToShow"]');
    resultsToShowInput.val(0);
    $(this).closest('.job-filter-panel').find('.update').click();
  });
