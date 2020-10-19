import { showServerSidePopup } from '../shared/popup';

export function initialiseVolunteerList() {
  $('.volunteer-list').on('click', '.add-credential', function (e) {
    e.preventDefault();
    const user = $(this).data('target-user');
    const group = $(this).data('target-group');
    const credential = $(this).data('credential');
    const url = `/api/volunteers/get-assign-credential-popup?u=${user}&g=${group}&c=${credential}`;
    const settings = {};
    showServerSidePopup(url, settings);
  });
}