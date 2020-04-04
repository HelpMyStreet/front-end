import "../sass/main.scss";
import notification from "./account/notification";
import "./shared/info-popup";
import "./shared/site-nav";

$(function() {
  $("#postcode_button").click(function(evt) {
    const postCode = $("#postcode").val();

    if (postCode) {
      $(this).width($(this).width());
      $(this).height($(this).height());

      $("#postcode_notcovered").hide();
      $("#postcode_covered").hide();
      $("#postcode_error").hide();
      $(this)
        .find(".text")
        .hide();
      $(this)
        .find(".loader")
        .show();

      fetch(`/api/postcode/${postCode}`)
        .then(resp => resp.json())
        .then(data => {
          if (data.addresses && data.addresses.length) $("#postcode_covered").show();
          else $("#postcode_notcovered").show();
        })
        .catch(err => {
          $("#postcode_error").show();
        })
        .finally(() => {
          $(this).width(null);
          $(this).height(null);
          $(this)
            .find(".text")
            .show();
          $(this)
            .find(".loader")
            .hide();
        });
    }
  });

  window.notification = notification;
});
