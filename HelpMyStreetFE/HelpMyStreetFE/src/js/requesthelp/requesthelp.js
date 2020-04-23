import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData, validatePostCode } from "../shared/validator";

$(() => {

	$("#postcode_button").on("click", async function (evt) {
		evt.preventDefault();

		$(".postcode__info, #postcode_invalid").hide();
		$(".postcode__info, #postcode_notcovered").hide();
		$(".postcode__info, #postcode_error").hide();

		$(".expanderDetails").slideUp();

		buttonLoad($(this));

		const postcode = $("input[name=postcode_search]").val();

		try {

			const responseLogResponse = await fetch(`/api/requesthelpapi/logRequest/${postcode}`);
			if (responseLogResponse.ok) {

				const responseLogResponseJson = await responseLogResponse.json();

				var logRequestValid = responseLogResponseJson.isSuccessful && responseLogResponseJson.hasContent;

				if (logRequestValid === false || responseLogResponseJson.content.fulfillable === false) {
					$(".postcode__info, #postcode_notcovered").show();
				}
				else {

					$("#requestId").val(responseLogResponseJson.content.requestID);
					$("#postcode").val(postcode);

					$(".expanderDetails").slideDown();
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
			lastname: (v) => v !== "" || "Please enter a last name",
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
		}

		$("#requesthelp_form").unbind('submit').submit(); // continue the submit unbind preventDefault

	});
});
