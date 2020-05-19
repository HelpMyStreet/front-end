export function initialiseRequests() {
  $('.job a.open').each((_, a) => {
    const el = $(a);
    const guid = el.attr('data-guid');
    el.on('click', (e) => {
      e.preventDefault();

      $(`#${guid}`).addClass('open');
      $(`#${guid} .job__detail`).slideToggle();
    });
  })

  $('.job a.close').each((_, a) => {
    const el = $(a);
    const guid = el.attr('data-guid');
    el.on('click', (e) => {
      e.preventDefault();

      $(`#${guid}`).removeClass('open');
      $(`#${guid} .job__detail`).slideToggle();
    });
  })
}
