import { buttonLoad, buttonUnload } from "../shared/btn";
import { validateFormData, validatePostCode } from "../shared/validator";
  
$(() => {

	$("#manual_address").on("click", function (evt) {
		evt.preventDefault();
		$(".expander").slideDown();
	});

	$("#address_finder").on("click", async function (evt) {
		evt.preventDefault();
		buttonLoad($(this));
		$("#address_selector").unbind("change");

		const postcode = $("input[name=postcode_search]").val();

		try {
			const resp = await fetch(`/api/postcode/checkCoverage/${postcode}`);
			console.log(resp);
			if (resp.ok) {						
				const response = await resp.json();
			
				console.log(response);	
					// also in response now is volunteerCount and championCount
				if (response.postCodeResponse.hasContent && response.postCodeResponse.isSuccessful) {
					$("select[name=address_selector]").html(
						response.postCodeResponse.content.addressDetails.reduce((acc, cur, i) => {
							const text = Object.keys(cur).reduce((tAcc, tCur) => {
								if (cur[tCur] != null) {
									tAcc += tAcc === "" ? "" : ", ";
									tAcc += `${cur[tCur]}`;
								}

								return tAcc;
							}, "");

							acc += `<option value="${i}">${text}</option>`;
							return acc;
						}, '<option value="" selected disabled hidden>Choose here</option>')
					);

					$("#address_selector").slideDown();
					$("select[name=address_selector]").on("change", function () {
						const id = $(this).children("option:selected").val();

						const address = response.postCodeResponse.content.addressDetails[id];

						$("input[name=address_line_1]").val(address.addressLine1);
						$("input[name=address_line_2]").val(address.addressLine2);
						$("input[name=city]").val(address.locality);
						$("input[name=postcode]").val(address.postcode);
						$(".expander").slideDown();
					});
				}
			}
		} catch (ex) {
			console.error(ex);
		}
		buttonUnload($(this));
	});

	$("#requesthelp_form").on("submit", function (event) {
		$(".expander").slideDown();

		event.preventDefault(); 

		let validHelpNeeded = true;

		var obj = $(this).serializeArray().reduce(function (acc, cur) {
			acc[cur.name] = cur.value;
			return acc;
		}, {});
		$(".error").hide();

		if (!obj["helpNeededArray"]) {
			validHelpNeeded = false;
			$("#helpNeeded-error").show();
		}

		const valid = validateFormData($(this), {
			firstname: (v) => v !== "" || "Please enter a first name",
			lastname: (v) => v !== "" || "Please enter a last name",
			email: (v) => v !== "" || "Please enter an email address",
			postcode: (v) => v !== "" || "Please enter a postcode",
			phonenumber: (v) =>
				v == "" ||
				((v.replace(" ", "").length === 10 || v.replace(" ", "").length === 11) && v[0] === "0") ||
				"Please enter a valid phone number",
			city: (v) =>
				(v.length > 2) ||
				"Please enter a valid city",
			address_line_1: (v) =>
				(v.length > 2) ||
				"Please enter a valid first line of your address",
		});

		let validForm = (valid && validHelpNeeded);

		let emailValid = true;
		let emailInput = $("input[name='email']");

		const emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

		if (!email) {
			emailInput.find("~ .error").text("Please provide an email address");
			emailValid = false;
		}
		if (typeof email != "string") {
			emailInput.find("~ .error").text("Please provide a valid email address");
			emailValid = false;
		}
		if (!emailRegex.test(email.textContent.toLowerCase())) {
			emailInput.find("~ .error").text("Please provide a valid email address");
			emailValid = false;
		}
   
		let postcodeValid;
		let postcodeInput = $("input[name='postcode']");
		event.preventDefault(); //this will prevent the default submit needed now we do a call to api
		if (validForm) { // avoid calling service when possible, so check if the form is valid first
			validatePostCode(postcodeInput.val()).then(function (response) {
				postcodeValid = response;
				if (!postcodeValid) {
					postcodeInput.find("~ .error").text("We could not validate that postcode, please check what you've entered and try again").show();
				} else {
					postcodeInput.find("~ .error").hide();
				}
			}).finally(function () {

				if (postcodeValid) {
					$("#requesthelp_form").unbind('submit').submit(); // continue the submit unbind preventDefault
				}
			});
		}
	});
});
