export function trackPageView(url) {
    $(window).on('load', function () {
        submitPageViewToGa(url, 100, 10)
    });
}

function submitPageViewToGa(url, retryDelayMs, attemptsToMake) {
    if (typeof ga !== 'undefined') {
        ga('set', 'page', url);
        ga('send', 'pageview');
    } else {
        if (attemptsToMake > 0) {
            console.warn('ga not defined. retrying...');
            window.setTimeout(submitPageViewToGa(url, retryDelayMs, attemptsToMake - 1), retryDelayMs);
        } else {
            console.warn('ga not defined. giving up...');
        }
    }
}