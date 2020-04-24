import { showLoadingSpinner } from "../states/loading";
import { buttonLoad, buttonUnload } from "../shared/btn";

export function initialiseStepFour() {
    $("#registration_form").on("submit", function (evt) {
        $('#championSelectError').hide();
        buttonLoad($('#submit_button'));        
        if (!$("input[name=street_champion]").is(":checked")) {
            buttonUnload($('#submit_button'));
            $('#championSelectError').show();
            $('#championSelectError').text("Please select an option")
            evt.preventDefault();
            return;
        } else {
            
            if ($("input[name=street_champion]:checked").val() == "true") {            
                if (getNumberOfPostcodesSelected() == 0) {
                    buttonUnload($('#submit_button'));
                    $('#championSelectError').show();
                    $('#championSelectError').text("You must select at least one postcode")
                    evt.preventDefault();
                    return;
                }
            }             
        }
    });

    $("input[name=street_champion]").on("change", function () {
        if ($(this).val() == "true") $(".expander").slideDown();
        else $(".expander").slideUp();
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