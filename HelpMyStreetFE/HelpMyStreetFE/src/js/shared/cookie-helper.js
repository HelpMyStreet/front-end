

export function intialiseCookieConsent() {
    $('body').ihavecookies({
        // Optional callback function when 'Accept' button is clicked
        onAccept: function () {
            var acceptedStatsCookie = $.fn.ihavecookies.preference('Statistics');
            if (acceptedStatsCookie == false) {
                deleteUneccsaryCookies(setCookie); // delete all cookies (GTM will add any back in based on the below cookies that are set)              
            }

            window.dataLayer = window.dataLayer || [];
            window.dataLayer.push({ 'event': 'cookie_clicked' });

            $("body").css("overflow", "visible");
        },
        // Optional callback function when modal appears
        onLoad: function () {
            $("body").css("overflow", "hidden");
        },
        // Array of cookie types for which to show checkboxes.
        // - type: Type of cookie. This is also the label that is displayed.
        // - value: Value of the checkbox so it can be easily identified in
        //          your application.
        // - description: Description for this cookie type. Displayed in
        //                title attribute.        
        title: 'This website uses cookies',
        message: ' We use cookies to help make our website usable and to understand how visitors interact with us. More information can be found in our <a href="/privacy-policy" target="_blank">Privacy Policy</a>. <br /> You can change or disable cookies at any time by changing your browser settings.',
        expires: 365,
        delay: 0,
        acceptBtnLabel: 'Accept All Cookies',
        cookieTypesTitle: 'Select cookies to accept:',
        cookieTypes: [
            {
                type: 'Statistics',
                value: 'Statistics',
                description: 'Cookies related to site visits, browser types, etc.'
            },

        ],

    });
    setTimeout(function () {
        $('#gdpr-cookie-advanced').click(function () {
            $('#gdpr-cookie-accept').html("Allow Selection")

        });
    }, 2300);
}

function deleteUneccsaryCookies(setCookieAfterDelete) {
    var neccesaryCookies = ['ARRAffinity', 'cookieControl', 'cookieControlPrefs', '.AspNetCore']

    var cookies = document.cookie.split(";");
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i];
        var eqPos = cookie.indexOf("=");
        var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        var isNeccesary = (neccesaryCookies.find(x => name.trim().startsWith(x.trim())) != undefined);
        if (cookie && !isNeccesary)
            document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
    }
    setCookieAfterDelete("optOutStats", "1", 365)
}
export function getCookie(name) {
    // Split cookie string and get all individual name=value pairs in an array
    var cookieArr = document.cookie.split(";");
    // Loop through the array elements
    for (var i = 0; i < cookieArr.length; i++) {
        var cookiePair = cookieArr[i].split("=");

        /* Removing whitespace at the beginning of the cookie name
        and compare it with the given string */
        if (name == cookiePair[0].trim()) {
            // Decode the cookie value and return
            return decodeURIComponent(cookiePair[1]);
        }
    }
    // Return null if not found
    return null;
}

export function setCookie(name, value, daysToLive) {
    // Encode value in order to escape semicolons, commas, and whitespace
    var cookie = name + "=" + encodeURIComponent(value);
    if (typeof daysToLive === "number") {
        /* Sets the max-age attribute so that the cookie expires
        after the specified number of days */
        cookie += "; max-age=" + (daysToLive * 24 * 60 * 60);

        document.cookie = cookie;
    }
}