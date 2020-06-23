import { showLoadingSpinner } from "../states/loading";
import { buttonLoad, buttonUnload } from "../shared/btn";
import { trackEvent } from "../shared/tracking-helper";

export function initialiseStepFour() {
    trackEvent("Registration flow", "View Step 4");

    $("#registration_form").on("submit", function (evt) {
        $('#championSelectError').hide();
        buttonLoad($('#submit_button'));        
        if (!$("input[name=street_champion]").is(":checked")) {
            buttonUnload($('#submit_button'));
            $('#championSelectError').show();
            $('#championSelectError').text("Please select one of the options above")
            evt.preventDefault();
            trackEvent("Registration flow", "Submit Step 4", "(Invalid)");
            return;
        } else {
            
            if ($("input[name=street_champion]:checked").val() == "true") {            
                if (getNumberOfPostcodesSelected() == 0) {
                    buttonUnload($('#submit_button'));
                    $('#championSelectError').show();
                    $('#championSelectError').text("Please select at least one postcode to cover as Street Champion")
                    evt.preventDefault();
                    trackEvent("Registration flow", "Submit Step 4", "(Invalid)");
                    return;
                }
            }             
        }
        trackEvent("Registration flow", "Submit Step 4", "(Valid)");
    });

    $("input[name=street_champion]").on("change", function () {
        if ($(this).val() == "true") {
            $(".expander").slideDown();
            trackEvent("Registration flow", "Select street champion", "yes");
        }
        else {
            $(".expander").slideUp();
            trackEvent("Registration flow", "Select street champion", "no");
        }
    });

    $(".postcode_checkbox").on("click", function (evt) {
        if (!$(this).find("input").is(":checked")) {
       
            if (getNumberOfPostcodesSelected() === 3) {
                evt.preventDefault();
            }
        }
    });

}


function getNumberOfPostcodesSelected() {
    const data = $("#registration_form").serializeArray();

    return data.reduce((acc, cur) => {
        acc += cur.name === "postcodes[]";
        return acc;
    }, 0);

}