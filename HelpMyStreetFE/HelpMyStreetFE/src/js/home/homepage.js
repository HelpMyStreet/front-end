import { initialiseNewsTicker } from "../components/news-ticker"
import { enableMaps, drawMap } from "../shared/maps";

$(".yt-video-placeholder").click(function () {
    var height = $(this).height();
    var width = $(this).width();
    $(this).html('<iframe style="min-width: ' + width + 'px; height: ' + height + 'px" src="https://www.youtube-nocookie.com/embed/BD--FjbDKp8?rel=0&amp;cc_load_policy=1&amp;modestbranding=1;autoplay=1" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen=""></iframe>');
});

$(function () {
  enableMaps().then(() => drawMap());
  initialiseNewsTicker();
});
