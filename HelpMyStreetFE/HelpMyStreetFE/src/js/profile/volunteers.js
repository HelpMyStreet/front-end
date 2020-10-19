import { showServerSidePopup } from '../shared/popup';
import { hmsFetch, fetchResponses } from '../shared/hmsFetch';

export function initialiseVolunteerList() {
  $('.volunteer-list').on('click', '.add-credential', async function (e) {
    e.preventDefault();
    const user = $(this).data('target-user');
    const group = $(this).data('target-group');
    const credential = $(this).data('credential');
    const url = `/api/volunteers/get-assign-credential-popup?u=${user}&g=${group}&c=${credential}`;

    let popup;
    const settings = {
      acceptCallbackAsync: async () => {
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
          return true;
        }
        return false;
      }
    };

    popup = await showServerSidePopup(url, settings);
  });
}