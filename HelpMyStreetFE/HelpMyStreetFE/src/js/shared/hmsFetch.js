// HMS Bespoke Fetch Function
// Takes fetch url, post/get data object (), options
// Options object allows setting timeout length, number of retries for errors and timeout
// Returns Promise that will resolve to a fetchResponse, and associated data.

import "isomorphic-fetch"

const defaultOptions = {
    timeOutLength: 5000,
    errorRetry: 3,
    timeOutRetry: 0
};

const fetchResponses = {
    TIMEOUT: "The fetch attempt timed out",
    BAD_FETCH: "The fetch attempt returned a runtime error",
    SERVER_ERROR: "The remote server returned an error",
    UNAUTHORISED: "The fetch request was unauthorised",
    BAD_REQUEST: "The fetch attempt was incorrectly formed",
    SERVER_NOT_FOUND: "The fetch endpoint could not be found",
    SUCCESS: "Fetch successful"
};

function sendNewRelicError(url, response, error) {
    var err = new Error(url + " - " + response + ": " + error)
    /*
    if (!newrelic) {
        console.error(err);
    }
    else {
        newrelic.noticeError(err);
    }
    */
    console.error(err);
};

async function tryFetch(url, data, options){
    let didTimeOut = false;
    let fetchResult = await new Promise(function(resolve, reject){
    const timeOut = setTimeout(function () {
            didTimeOut = true;
            if (options.timeOutRetry > 0){
                options.timeOutRetry = options.timeOutRetry - 1;
                resolve(tryFetch(url,data,options));
            } else {
                sendNewRelicError(url, fetchResponses.TIMEOUT);
                resolve({fetchResponse: fetchResponses.TIMEOUT});
            }
        }, options.timeOutLength);

        fetch(url, data)
            .then(response => {
                clearTimeout(timeOut);
                switch (response.status) {
                    case 200:
                        resolve({fetchResponse: fetchResponses.SUCCESS, fetchPayload: response.json()});
                        break;
                    case 400:
                        var fetchError = fetchResponses.BAD_REQUEST;
                        break;
                    case 401:
                        var fetchError = fetchResponses.UNAUTHORISED;
                        break;
                    case 404:
                        var fetchError = fetchResponses.SERVER_NOT_FOUND;
                        break;
                    case 500:
                        if (options.errorRetry > 0) {
                            options.errorRetry = options.errorRetry - 1;
                            resolve(tryFetch(url, data, options));
                        } else {
                            var fetchError = fetchResponses.SERVER_ERROR;
                        }
                        break;
                    default:
                        resolve({ fetchResponse: response.status, fetchPayload: response.json() });

                }
                if (fetchError) {
                    sendNewRelicError(url, fetchError, response.statusText);
                    resolve({ fetchResponse: fetchError });
                }
            })
            .catch(err => {
                if (didTimeOut) return;
                sendNewRelicError(url, fetchResponses.BAD_FETCH, err);
                resolve({fetchResponse: fetchResponses.BAD_FETCH});
            });
        });
    return fetchResult;
}

async function hmsFetch(url, data, options) {

    var _options = defaultOptions;
    if (options) { Object.assign(_options, options) };

    return tryFetch(url, data, _options)
};


    


export { hmsFetch, fetchResponses };