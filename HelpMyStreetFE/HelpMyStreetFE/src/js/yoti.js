

$(() => {

    var getParameterByName = function (name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    }

    var processYoti = async function (thisToken) {
        var response = await fetch("/yoti/ValidateToken" + "?token=" + thisToken);
        console.log(response);
        if (response.status == 200) {
            window.location.href = "/yoti/AuthSuccess";
        } else {
            window.location.href = "/yoti/AuthFailed";            
        }
    }

    var urlToken = getParameterByName("token");
    if (urlToken) {   
        processYoti(urlToken)
    } else {
        if (initObj) {
            window.Yoti.Share.init({
                elements: [
                    {
                        domId: initObj.domId,
                        scenarioId: initObj.scenarioId,
                        clientSdkId: initObj.clientSdkId,
                        button: {
                            label: "Open Yoti",
                            align: "center",
                            width: "full", // "auto"
                        },
                        modal: {
                            zIndex: 9999,
                        },
                        shareComplete: {
                            closeDelay: 4000, // default to 4000, min of 500 - max of 10000
                            tokenHandler: async (token, done) => {
                                processYoti(token);
                                done();
                            },
                        },
                    },
                ],
            });
        } else {
            throw new Error("initObj is null");
        }
    }
});
