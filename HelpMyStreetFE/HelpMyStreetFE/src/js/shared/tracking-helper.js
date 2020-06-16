const gaRetryDelayMs = 250;
const gaRetryAttempts = 10;

export function trackPageView(url) {
    submitPageViewToGa(url, gaRetryDelayMs, gaRetryAttempts)
}

function submitPageViewToGa(url, retryDelayMs, attemptsToMake) {
    if (typeof ga !== 'undefined') {
        ga('gtm1.set', 'page', url);
        ga('gtm1.send', 'pageview');
    } else {
        if (attemptsToMake > 0) {
            console.warn('ga not defined; retrying');
            window.setTimeout(submitPageViewToGa(url, retryDelayMs, attemptsToMake - 1), retryDelayMs);
        } else {
            console.warn('ga not defined');
        }
    }
}