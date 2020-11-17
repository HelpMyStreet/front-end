import { hmsSubmit, fetchResponses } from "../shared/hmsFetch";
import { loadFeedbackComponents } from "../profile/requests";

const toggleButtons = document.querySelectorAll(".btn__toggle-show");

toggleButtons.forEach((btn) => {
  const form = $(btn).closest('form');
  btn.addEventListener("click", (e) => {
    e.preventDefault();
    const target = form.find('#' + btn.getAttribute("data-target-item"));
    target.toggleClass("filter--show");

    if (target.hasClass("filter__list-wrapper")) {
      // Div containing all filters
      if (target.hasClass("filter--show")) {
        lockWindowScroll();
      } else {
        unlockWindowScroll();
      }
    } else {
      // Just one list
      updateFilterSummary(target);
      form.find(".btn--apply-filter").removeClass("applied").removeClass("disabled");
    }
  });
});

// Apply filter button
const applyButtons = document.querySelectorAll(".btn--apply-filter");
applyButtons.forEach((btn) => {
  const form = $(btn).closest('form');
  btn.addEventListener("click", async (e) => {
    e.preventDefault();
    //buttonLoad($(b)); // Removing for now due to size jumping bug
    btn.classList.remove("applied");
    updateFilterSummaries();

    if (loadRequests(form)) {
      form.find(".filter--show").removeClass("filter--show");
      btn.classList.add("applied");
      unlockWindowScroll();
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
  var response = await hmsSubmit('/api/request-help/get-filtered-jobs', form);
  if (response.fetchResponse == fetchResponses.SUCCESS) {
      $(form).closest('.job-filter-panel').find('.job-filter-results-panel .job-list').html(await response.fetchPayload);
      loadFeedbackComponents();
    return true;
  }
  return false;
}

$(function() {
  updateFilterSummaries();
});

function updateFilterSummaries() {
  $('.filter__list__category').each(function () { updateFilterSummary(this); });
}

function updateFilterSummary(list) {
  let summary = "";
  const inputs = $(list).find('input');
  if (inputs.first().is('[type="checkbox"]')) {
    if (!$(inputs).is(':not(:checked)')) {
      summary = 'Select all';
    } else {
      summary = $.map($(inputs).filter(':checked'), function (i) {
        return $(list).find('label[for="' + i.id + '"]').first().html();
      }).join(', ');
    }
  } else if (inputs.first().is('[type="radio"]')) {
    const selectedInput = $(inputs).filter(':checked').first();
    summary = $(list).find('label[for="' + selectedInput.attr('id') + '"]').first().html()
  }
  $(list).find('.filter__list__category__summary').html(summary);
}

function lockWindowScroll() {
  document.body.style.top = `-${window.scrollY}px`;
  document.body.style.position = 'fixed';
}

function unlockWindowScroll() {
  if (document.body.style.position == 'fixed') {
    const scrollY = document.body.style.top;
    document.body.style.position = '';
    document.body.style.top = '';
    window.scrollTo(0, parseInt(scrollY || '0') * -1);
  }
}