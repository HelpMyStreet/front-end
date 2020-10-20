import { showServerSidePopup } from '../shared/popup';
import { hmsFetch, fetchResponses } from '../shared/hmsFetch';
import { datepickerLoad, validateDate, dateValidationSchemes } from '../shared/date-picker';

export function initialiseVolunteerList() {
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
        if (!validateCredentialForm()) {
          return 'Please check your entries above and try again.';
        }

        const formData = $(popup).find('form').serializeArray();
        let dataToSend = {};
        formData.forEach((d) => {
          dataToSend[d.name] = d.value;
        });

        var fetchRequestData = {
          method: 'POST',
          body: JSON.stringify(dataToSend),
          headers: { 'Content-Type': 'application/json' },
        };
        var response = await hmsFetch(`/api/volunteers/put-volunteer-credential?u=${user}&g=${group}&c=${credential}`, fetchRequestData);
        if (response.fetchResponse == fetchResponses.SUCCESS) {
          $(el).replaceWith('<span class="added">Added</span>');
          return true;
        }
        return 'Oops, we couldn’t add that credential at the moment.';
      }
    };

    popup = await showServerSidePopup(url, settings);
    datepickerLoad('datepicker', 'datepicker-error', dateValidationSchemes.FUTURE_DATES);
  });

  intialiseCredentialPopupTiles();
}

var intialiseCredentialPopupTiles = function () {
  console.log('init');
  $('body').on('click', '.tiles__tile', function (el) {
    console.log('click');
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

var validateCredentialForm = function () {
  $('#datepicker-error').hide();
  const dateValue = $('input[name="ValidUntil"]').val();
  if (dateValue.length == 0) {
    $('#datepicker-error').text('Please select an option or enter a date').show();
    return false;
  }
  if (dateValue !== 'Null' && !validateDate(dateValue, 'datepicker', 'datepicker-error', dateValidationSchemes.FUTURE_DATES)) {
    return false;
  }
  return true;
};