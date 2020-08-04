export function initialiseSliders() {

    var sliders = document.getElementsByClassName('slides');

    sliders.forEach(slides => {
        var numberOfFrames = 0;
        var totalWidth = 0;
        slides.querySelectorAll('div').forEach(s => { numberOfFrames++; totalWidth += $(s).outerWidth(true); });
        const visibleWidth = $(slides).width();
        var scrollTarget;
        if (numberOfFrames % 2 == 1) {
            // odd - centre centre image
            scrollTarget = Math.floor(totalWidth / 2 - visibleWidth / 2);
        } else {
            // even - centre left-of-centre image
            const frameWidth = $(slides).find('div:nth-child(2)').first().outerWidth(true); // use second frame; first has non-typical margin
            scrollTarget = Math.floor(totalWidth / 2 - visibleWidth / 2 - frameWidth / 2);
        }
        $(slides).scrollLeft(scrollTarget);
    });
}