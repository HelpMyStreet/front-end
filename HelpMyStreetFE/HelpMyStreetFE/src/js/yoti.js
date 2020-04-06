$(() => {
  if (initObj) {
    window.Yoti.Share.init({
      elements: [
        {
          domId: initObj.domId,
          scenarioId: initObj.scenarioId,
          clientSdkId: initObj.clientSdkId,
          button: {
            label: "Use Yoti",
            align: "center", // "left" | "right"
            width: "full", // "auto"
          },
          modal: {
            zIndex: 9999, // default to 9999, min of 0 - max of 2147483647
          },
          shareComplete: {
            closeDelay: 4000, // default to 4000, min of 500 - max of 10000
            tokenHandler: async (token, done) => {
              var response = await fetch("/yoti/ValidateToken", {
                method: "post",
                body: token,
              });
              console.log(response);
              done(); //done() will overwrite the closeDelay
            },
          },
        },
      ],
    });
  } else {
    throw new Error("initObj is null");
  }
});
