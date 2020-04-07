$(() => {
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
              var response = await fetch("/yoti/ValidateToken", {
                method: "POST",
                headers: {
                  "Content-Type": "application/json",
                },
                body: JSON.stringify({ Token: token }),
              });

              if (response.status == 200) {
                done();
                window.location.href = "/yoti/AuthSuccess";
              } else {
                window.location.href = "/yoti/AuthFailed";
              }
            },
          },
        },
      ],
    });
  } else {
    throw new Error("initObj is null");
  }
});
