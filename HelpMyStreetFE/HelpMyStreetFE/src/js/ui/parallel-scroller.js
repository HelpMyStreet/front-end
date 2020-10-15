export function initialiseParallelScrollers() {
  $('.parallel-scroller').scroll(function (e) {
    $('.parallel-scroller').scrollLeft(e.target.scrollLeft);
  });
};

