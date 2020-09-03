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
    //buttonLoad($(b)); // Removing for now due to size jumping bug
    b.classList.remove("applied");

    if (loadRequests($(b).closest("form"))) {
      const g = document.getElementById(b.getAttribute("data-target-group"));
      g.querySelectorAll('.group--filters-jobs').forEach((t) => {
        t.classList.remove("filter--show");
      });
      b.classList.add("applied");
    }

    //buttonUnload($(b));
    return false;
  });
});


// Initialise select all checkboxes
$('.job-filter-panel input[type="checkbox"].filter-select-all').each(function () {
  const inputsInWrapper = $(this).closest('.form-group__wrapper').find('input[type="checkbox"]:not(".filter-select-all")');
  $(this).prop('checked', !$(inputsInWrapper).is(':not(:checked)'));
});

// Update select all checkboxes
$('.job-filter-panel').on('click', 'input[type="checkbox"]', function () {
  const inputsInWrapper = $(this).closest('.form-group__wrapper').find('input[type="checkbox"]:not(".filter-select-all")');
  if (this.classList.contains('filter-select-all')) {
    inputsInWrapper.prop('checked', $(this).is(':checked'));
  } else {
    const selectAllInput = $(this).closest('.form-group__wrapper').find('input[type="checkbox"].filter-select-all');
    $(selectAllInput).prop('checked', !$(inputsInWrapper).is(':not(:checked)'));
  }
});

$('.job-filter-panel').on('click', '.show-more-jobs', function (e) {
  e.preventDefault();
  const form = $(this).closest('.job-filter-panel').find('form');
  const resultsToShowInput = $(form).find('input[name="ResultsToShow"]');
  const resultsToShowIncrementInput = $(form).find('input[name="ResultsToShowIncrement"]');
  resultsToShowInput.val(parseInt(resultsToShowInput.val()) + parseInt(resultsToShowIncrementInput.val()));
  loadRequests(form);
});

$('.job-filter-panel').on('click', '.show-all-jobs', function (e) {
  e.preventDefault();
  const form = $(this).closest('.job-filter-panel').find('form');
  const resultsToShowInput = $(form).find('input[name="ResultsToShow"]');
  resultsToShowInput.val(0);
  loadRequests(form);
});

async function loadRequests(form) {
  const formData = $(form).serializeArray();
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
    return true;
  }
  return false;
}
