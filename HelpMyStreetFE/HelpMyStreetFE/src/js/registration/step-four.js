import { showLoadingSpinner } from "../states/loading";
import { buttonLoad, buttonUnload } from "../shared/btn";

export function initialiseStepFour() {

    $("#registration_form").on("submit", function (evt) {        
        buttonLoad($('#submit_button'));
    });

    $("input[name=street_champion]").on("change", function () {
        if ($(this).val() == "true") $(".expander").slideDown();
        else $(".expander").slideUp();
    });

    $(".postcode_checkbox").on("click", function (evt) {
        if (!$(this).find("input").is(":checked")) {
            const data = $("#registration_form").serializeArray();

            const numPostcodes = data.reduce((acc, cur) => {
                acc += cur.name === "postcodes[]";
                return acc;
            }, 0);

            if (numPostcodes === 3) {
                evt.preventDefault();
            }
        }
    });

}


