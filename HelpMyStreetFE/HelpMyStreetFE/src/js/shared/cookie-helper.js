export function intialiseCookieConsent() {
    $('body').ihavecookies({
        // Optional callback function when 'Accept' button is clicked
        onAccept: function () {
            // Do whatever you need to here...
        },

        // Array of cookie types for which to show checkboxes.
        // - type: Type of cookie. This is also the label that is displayed.
        // - value: Value of the checkbox so it can be easily identified in
        //          your application.
        // - description: Description for this cookie type. Displayed in
        //                title attribute.        
        title: 'This website uses cookies',
        message: ' We use cookies to help make our website usable and to understand how visitors interact with us. More information can be found in our <a href="/privacy-policy" target="_blank">Privacy Policy</a>. <br /> You can change or withdraw your consent at any time by changing your browser settings.',
        expires: 365,
        moreInfoLabel: '',
        
        cookieTypes: [         
            {
                type: 'Statistics',
                value: 'Statistics',
                description: 'Cookies related to site visits, browser types, etc.'
            },
        
        ],

    });
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