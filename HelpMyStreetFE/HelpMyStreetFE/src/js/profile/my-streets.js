export function initialiseMyStreet() {    
    $('.profile__mystreet__header').click(function () {
        var details = $(this).find("~ .profile__mystreet__details")        
        details.toggleClass("dnone");

        if (details.hasClass("dnone")) {
            
            $(this).find(".profile__mystreet__header__arrow > span").removeClass("profile__mystreet__header__arrow--down")
            $(this).find(".profile__mystreet__header__arrow > span").addClass("profile__mystreet__header__arrow--up")

        } else {
            $(this).find(".profile__mystreet__header__arrow > span").removeClass("profile__mystreet__header__arrow--up")
            $(this).find(".profile__mystreet__header__arrow > span").addClass("profile__mystreet__header__arrow--down")
          
        }                       
    })
}