$(function () {
    $(".feedback-message").slick({
        infinitie: true,
        slidesToShow: 3,
        slidesToScroll: 3,
        autoplay: true,
        autoplayspeed: 20000,
        dots: true,
        arrows: true,

        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: true
                },
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: true,
                    autoplay: true,
                    autoplayspeed: 2000
                }
            }
        ]
    });
});