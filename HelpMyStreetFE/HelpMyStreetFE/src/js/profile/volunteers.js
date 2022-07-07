import { showServerSidePopup } from '../shared/popup';
import { hmsSubmit, fetchResponses } from '../shared/hmsFetch';
import { datepickerLoad, validateDate, dateValidationSchemes } from '../shared/date-picker';
import { validateFormData } from '../shared/validator'

export function initialiseVolunteerList() {
  initialiseAddCredentialLinks();
  initialiseWhatIsThisLinks();
  intialiseCredentialPopupTiles();
}

var initialiseAddCredentialLinks = function () {
  $('.volunteer-list').on('click', '.add-credential', async function (e) {
    e.preventDefault();
    const el = this;
    const user = $(this).data('target-user');
    const group = $(this).data('target-group');
    const credential = $(this).data('credential');
    const url = `/api/volunteers/get-assign-credential-popup?u=${user}&g=${group}&c=${credential}`;

    let popup;
    const settings = {
      acceptCallbackAsync: async () => {
        if (!validateCredentialForm($(popup).find('form'))) {
          return 'Please check your entries above and try again.';
        }

        var response = await hmsSubmit(`/api/volunteers/put-volunteer-credential?u=${user}&g=${group}&c=${credential}`, $(popup).find('form'));
        if (response.fetchResponse == fetchResponses.SUCCESS) {
          $(el).replaceWith('<span class="added">Added</span>');
          return true;
        }
        return "Oops, we couldn't add that credential at the moment.";
      }
    };

    popup = await showServerSidePopup(url, settings);
    datepickerLoad($('#datepicker'), $('#datepicker-error'), dateValidationSchemes.FUTURE_DATES);
  });
};

var initialiseWhatIsThisLinks = function () {
    $('.volunteer-list').on('click', '.what-is-this', async function (e) {
        e.preventDefault();

        const group = $(this).data('target-group');
        const credential = $(this).data('credential');
        const item = $(this).data('item');

        if (credential != undefined) {
            await showServerSidePopup(`/api/volunteers/get-what-is-this-credential-popup?g=${group}&c=${credential}`);
        } else {
            await showServerSidePopup(`/api/volunteers/get-what-is-this-credential-popup?g=${group}&i=${item}`);
        }
    });
};

var intialiseCredentialPopupTiles = function () {
  $('body').on('click', '.tiles__tile', function (el) {
    $('.tiles__tile').removeClass('selected');
    let showDatePicker = $(this).data('show-date-picker');
    if (showDatePicker == 'True') {
      $('#date-picker').show();
    } else {
      $('#date-picker').hide();
    }
    $(this).addClass('selected');
    $('input[name="ValidUntil"]').val($(this).data('value'));
    $('#datepicker-error').hide();
  });
};

var validateCredentialForm = function (form) {
  $('#datepicker-error').hide();
  const valid = validateFormData(form, {
    Reference: (v) => v.length < 100 || "Reference number must be fewer than 100 charachters in length",
    Notes: (v) => v.length < 200 || "Reference number must be fewer than 200 charachters in length",
  });
  const dateValue = $('input[name="ValidUntil"]').val();
  if (dateValue.length == 0) {
    $('#datepicker-error').text('Please select an option or enter a date').show();
    return false;
  }
  if (dateValue !== 'Null' && !validateDate(dateValue, $('#datepicker-error'), dateValidationSchemes.FUTURE_DATES)) {
    return false;
  }
  return valid;
};