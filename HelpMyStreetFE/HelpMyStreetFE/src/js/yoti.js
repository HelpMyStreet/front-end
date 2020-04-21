

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

    var urlToken = getParameterByName("token");
    var userId = getParameterByName("u");

    alert("User ID:" + userId);

    var processYoti = async function (thisToken) {
        $('.yoti__auth__button').hide();
        $('.yoti__auth__loading').css("visibility", "visible");
        $('.yoti__auth__loading').css("height", "100%");
        var response = await fetch("/yoti/ValidateToken" + "?token=" + thisToken + "&u=" + userId);
        if (response.status == 200) {
            window.location.href = "/yoti/AuthSuccess?u=" + userId;
        } else {            
            window.location.href = "/yoti/AuthFailed?u=" + userId;
        }
    }

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
