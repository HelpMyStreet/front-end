const gaRetryDelayMs = 250;
const gaRetryAttempts = 10;

export function trackPageView(url) {
    submitPageViewToGa(url, gaRetryDelayMs, gaRetryAttempts)
}

export function trackEvent(eventCategory, eventAction, eventLabel, eventValue) {
    submitEventToGa(eventCategory, eventAction, eventLabel, eventValue, gaRetryDelayMs, gaRetryAttempts)
}

function submitPageViewToGa(url, retryDelayMs, attemptsToMake) {
    if (typeof ga !== 'undefined') {
        ga('gtm1.set', 'page', url);
        ga('gtm1.send', 'pageview');
    } else {
        if (attemptsToMake > 0) {
            window.setTimeout(function () { submitPageViewToGa(url, retryDelayMs, attemptsToMake - 1); }, retryDelayMs);
        } else {
            console.warn('ga not defined');
        }
    }
}

function submitEventToGa(eventCategory, eventAction, eventLabel, eventValue, retryDelayMs, attemptsToMake) {
    if (typeof ga !== 'undefined') {
        ga('gtm1.send', 'event', eventCategory, eventAction, eventLabel, eventValue);
    } else {
        if (attemptsToMake > 0) {
            window.setTimeout(function () { submitEventToGa(eventCategory, eventAction, eventLabel, eventValue, retryDelayMs, attemptsToMake - 1); }, retryDelayMs);
        } else {
            console.warn('ga not defined');
        }
    }
}