import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData, validatePostCode } from "../shared/validator";

function validatePrivacyAndTerms() {
	// requires checking of two or more inputs at the same time, so cant use the validateFormData.
	$('.termsprivacy').hide();
	let privacy = $("input[name='privacy_notice']").is(":checked");
	let terms = $("input[name='terms_and_conditions']").is(":checked");
	var errorText = "";
	privacy == false && terms == false ? errorText = "Please tick to indicate that you acknowledge our Privacy Policy and accept our Terms and Conditions." : "";
	privacy == true && terms == false ? errorText = "Please tick to confirm that you agree to the Help My Street <a href='/terms-conditions'>Terms and Conditions</a>" : "";
	privacy == false && terms == true ? errorText = "Please tick to confirm that you acknowledge the Help My Street <a href='/privacy-policy'>Privacy Notice</a>" : "";

	$('.termsprivacy').show();
	$('.termsprivacy').html(errorText);

	if (errorText !== "") {
		return false;
	}
	return true;
}

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
							$("#streetchampionCoverage .postcode__info__message__text").html("<p>Great! There are volunteers in that area! Please go ahead and tell us a litte more.</p>");
						} else {
							$("#streetchampionCoverage .postcode__info__message__text").html("<p>We've just launched HelpMyStreet and we're building our network across the country. We're working hard to ensure we have local volunteers in this area who can get the right help to the right people.</p>"
								+ "<p>We'll do all we can to find someone to complete your request, but this may take a few days. Can we go ahead and do that for you?</p>");
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

		if (validatePrivacyAndTerms() === false) {
			$("#general-error").show();
			return;
		} 
		const valid = validateFormData($(this), {
			firstname: (v) => v !== "" || "Please enter a first name",
			email: (v) => (v == "") || validateEmail(v) ||
				"Please enter a valid email address",
			phonenumber: (v) =>
				v == "" ||
				((v.replace(" ", "").length === 10 || v.replace(" ", "").length === 11) && v[0] === "0") ||
				"Please enter a valid phone number",
			phonenumber: (v, d) =>
				(v !== "") || (d.email !== "") || "Please enter an email address or a phone number",
		
		});

		let validForm = (valid && validHelpNeeded);

		if (!validForm) {
			$("#general-error").show();
		} else {
			$("#requesthelp_form").unbind('submit').submit(); // continue the submit unbind preventDefault
		}
	});
});


function validateEmail(email) {
	var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
	return re.test(email);
}
