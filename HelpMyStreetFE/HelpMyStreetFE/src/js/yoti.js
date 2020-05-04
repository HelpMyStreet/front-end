

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
  

    var processYoti = async function (thisToken, userId) {
        $('.yoti__auth__button').hide();
        $('.yoti__auth__loading').css("visibility", "visible");
        $('.yoti__auth__loading').css("height", "100%");
        var response = await fetch("/yoti/ValidateToken?token=" + thisToken + "&u=" + userId);
        if (response.status == 200) {
            window.location.href = "/yoti/AuthSuccess";
        } else {            
            window.location.href = "/yoti/AuthFailed?u=" + userId;
        }
    }

    if (initObj) {
        if (urlToken) {
            processYoti(urlToken, initObj.userId)
        } else {
            window.Yoti.Share.init({
                elements: [
                    {
                        domId: initObj.domId,
                        scenarioId: initObj.scenarioId,
                        clientSdkId: initObj.clientSdkId,
                        type: "inline",
                        qr: {
                            title: " Scan with the Yoti app"
                        },
                        modal: {
                            zIndex: 9999,
                        },
                        shareComplete: {    
                            tokenHandler: async (token, done) => {
                                processYoti(token, initObj.userId);                             
                            },
                        },
                    },
                ],
            });
        }
    }else {
            throw new Error("initObj is null");
        }    
});
