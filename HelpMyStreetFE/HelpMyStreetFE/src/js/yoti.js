

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
  
  

    if (initObj) {
        if (urlToken) {
            processYoti(urlToken, initObj.userId)
        }                  
    }else {
            throw new Error("initObj is null");
        }    
});


var processYoti = async function (thisToken, userId) {
    $('#overlay').show();
    $('.loading-overlay').show();
    var response = await fetch("/yoti/ValidateToken?token=" + thisToken + "&u=" + userId);
    if (response.status == 200) {
        window.location.href = "/Account";
    } else {
        var event = document.createEvent('Event');
        event.initEvent('failed-auth', true, true);
        document.getElementById('verification-panel').dispatchEvent(event);
        $('#overlay').hide();
        $('.loading-overlay').hide();    
    }
}
var yoti;
export function initialiseYoti() {
    if (initObj) {
        if (yoti) {
            yoti.destroy();
        }
       yoti = window.Yoti.Share.init({
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
                            done();
                        },
                    },
                },
            ],
       });

    }else {
            throw new Error("initObj is null");
        }   
}
