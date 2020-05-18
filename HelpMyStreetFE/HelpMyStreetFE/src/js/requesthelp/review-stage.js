
export var reviewStage = {
    communicationNeeds: { val: null, errorSpan: "e-communication-needs" },
    helperAdditionalDetails: { val: null, errorSpan: "e-helper-additional-details" },

    getLatestValues: function () {          
        this.communicationNeeds.val = $('textarea[name="communication-needs"]').val();                
        this.helperAdditionalDetails.val = $('textarea[name="helper-additional-details"]').val();        
    },
    validate: function () {
        return true;
    }
};


export function intialiseReviewStage(obj) {    
    loadSelectedActivity(obj.request.selectedActivity.val);
    loadSelectedFor(obj.request.selectedFor.val);
    loadDescripton(obj.request)
    loadPersonalDetails(obj.detail);   
}

var loadSelectedActivity = function (id) {    
    $('#selected-activity').html($('#' + id).parent().html()).find("#" + id).removeClass("selected");
}

var loadSelectedFor = function (id) {
    $('#selected-request-for').html($('#' + id).parent().html()).find("#" + id).removeClass("selected");
}


var loadDescripton = function (request) {
    if (request.additonalHelpDetail.val) {
        $('#review-additional-info').text(request.additonalHelpDetail.val);
    } else {
        $('#review-additional-info').text("No additional information supplied");
    }    
    $('#review-critical-request').text(request.selectedHealthWellBeing.val == true ? "Yes" : "No");
    if (request.selectedTime.id == "time_5") {
        let plural = "s"
        if (request.selectedTime.val == 1) {
            plural = "";
        }
        $('#review-timeframe').text("Within " + request.selectedTime.val + " Day" + plural)
    } else {
        $('#review-timeframe').text($('#' + request.selectedTime.id).text());
    }
}

var loadPersonalDetails = function (detail) {
    $('#review-your-name').text(detail.yourDetails.firstname.val + " " + detail.yourDetails.lastname.val);

    let address = ""
    if (detail.yourDetails.addressLine1) {
        address = detail.yourDetails.address.addressLine1.val + ", " + detail.yourDetails.address.locality.val + ", ";
    }

    $('#review-your-address').text(address +  detail.yourDetails.address.postcode.val)
    $('#review-your-email').text(detail.yourDetails.email.val)
    $('#review-your-mobile').text(detail.yourDetails.mobilenumber.val ? detail.yourDetails.mobilenumber.val : "Not Provided")
    $('#review-your-alternate').text(detail.yourDetails.altnumber.val ? detail.yourDetails.altnumber.val : "Not Provided");

    if (detail.onBehalf == false) {
        $('#review-your-details').removeClass("sm4");
        $('#review-your-details').addClass("sm8");
        $('#review-their-details').hide()
    } else {
        if ($('#review-your-details').hasClass("sm8")) {
            $('#review-your-details').removeClass("sm8");
            $('#review-your-details').addClass("sm4");
            $('#review-their-details').show();
        }

        $('#review-their-name').text(detail.theirDetails.firstname.val + " " + detail.theirDetails.lastname.val);
        $('#review-their-address').text(detail.theirDetails.address.addressLine1.val + ", " + detail.theirDetails.address.locality.val + ", " + detail.theirDetails.address.postcode.val)
        $('#review-their-email').text(detail.theirDetails.email.val ? detail.theirDetails.email.val: "Not Provided")
        $('#review-their-mobile').text(detail.theirDetails.mobilenumber.val ? detail.theirDetails.mobilenumber.val : "Not Provided")
        $('#review-their-alternate').text(detail.theirDetails.altnumber.val ? detail.theirDetails.altnumber.val : "Not Provided");
    }


}





export function onDirectToRequestClick(callback) {
    return $('.to-request').click(function () {
        callback();
    });
         
}

export function onDirectToDetailClick(callback) {
    return $('.to-details').click(function () {
        callback();
    });
}

