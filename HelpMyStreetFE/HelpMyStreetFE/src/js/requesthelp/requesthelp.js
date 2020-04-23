import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData, validatePostCode } from "../shared/validator";

$(() => {

	$("#postcode_button").on("click", async function (evt) {
		evt.preventDefault();

		$(".postcode__info, #postcode_invalid").hide();
		$(".postcode__info, #postcode_notcovered").hide();
		$(".postcode__info, #postcode_error").hide();
		$(".postcode__info, #postcode_invalid").hide();
		$(".postcode__info, #streetchampionCoverage").hide();
		
		$(".expanderDetails").slideUp();

		buttonLoad($(this));

		const postcode = $("input[name=postcode_search]").val();

		try {

			const responseLogResponse = await fetch(`/api/requesthelpapi/logRequest/${postcode}`);
			if (responseLogResponse.ok) {
			
				const responseLogResponseJson = await responseLogResponse.json();

				var logRequestValid = responseLogResponseJson.isSuccessful && responseLogResponseJson.hasContent;
				console.log(responseLogResponseJson.content);
				if (logRequestValid === false) {
					$(".postcode__info, #postcode_notcovered").show();
				}
				else if (responseLogResponseJson.content.fulfillable == 1) {			 //invalid postcode		
					$(".postcode__info, #postcode_invalid").show();
				}
				else {

					if (responseLogResponseJson.content.fulfillable == 4 || responseLogResponseJson.content.fulfillable == 6) {// pass to street champion or // manuel defer
						var hasStreetChamp = responseLogResponseJson.content.fulfillable == 4;
						$(".postcode__info, #streetchampionCoverage").show();
						if (hasStreetChamp) {
							$("#streetchampionCoverage .postcode__info__message__text").text("Great! There are volunteers in that area! Please go ahead and tell us a litte more.");
						} else {
							$("#streetchampionCoverage .postcode__info__message__text").text("We've just launched HelpMyStreet and we're building our network across the country. We're working hard to ensure we have local volunteers in this area who can get the right help to the right people."
								+ "We'll do all we can to find someone to complete your request, but this may take a few days. Can we go ahead and do that for you? Please tell us more about your request so we can find the right person to help you.");
						}
						$("#streetchampionCoverage").show();
						$('#continueRequest').click(function () {
							$('#hasStreetChampion').val(hasStreetChamp);
							$(".postcode__info, #streetchampionCoverage").hide();
							$("#requestId").val(responseLogResponseJson.content.requestID);
							$("#postcode").val(postcode);
							$(".expanderDetails").slideDown();
						})
					} 

					
				}
			}
		} catch (ex) {
			console.error(ex);
			$(".postcode__info, #postcode_error").show();
		}
		buttonUnload($(this));
	});

	$("#requesthelp_form").on("submit", function (event) {

		event.preventDefault();

		let validHelpNeeded = true;

		var obj = $(this).serializeArray().reduce(function (acc, cur) {
			acc[cur.name] = cur.value;
			return acc;
		}, {});
		$(".error").hide();

		if (!obj["help-needed-array"]) {
			validHelpNeeded = false;
			$("#help-needed-array-error").show();
		}

		const valid = validateFormData($(this), {
			firstname: (v) => v !== "" || "Please enter a first name",
			email: (v) => v !== "" ||
				RegExp('/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/').test(v) ||
				"Please enter a valid email address",
			phonenumber: (v) =>
				v == "" ||
				((v.replace(" ", "").length === 10 || v.replace(" ", "").length === 11) && v[0] === "0") ||
				"Please enter a valid phone number",
		});

		let validForm = (valid && validHelpNeeded);

		if (!validForm) {
			$("#general-error").show();
		} else {
			$("#requesthelp_form").unbind('submit').submit(); // continue the submit unbind preventDefault
		}
	});
});
