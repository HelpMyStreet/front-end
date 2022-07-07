export const dateValidationSchemes = {
    OVER_18: "Permit only dates over 18 years ago",
    FUTURE_DATES: "Permit only dates in the future",
    FUTURE_DATES_6M: "Permit only dates in next 6 months",
}

export function datepickerLoad(id, errorId, dateValidationScheme = dateValidationSchemes.OVER_18) {
    let datepicker = document.getElementById(id);


    datepicker.addEventListener("focusout", function (e) {
      validateDate(this.value, errorId, dateValidationScheme);
    });

     // event listner to add slashes whilst entering input
    datepicker.addEventListener('input', function (e) {        
        var input = this.value;
        if (/\D\/$/.test(input)) input = input.substr(0, input.length - 3);
        var values = input.split('/').map(function (v) {
            return v.replace(/\D/g, '')
        });
        if (values[0]) values[0] = checkValue(values[0], 31, values.length > 1);
        if (values[1]) values[1] = checkValue(values[1], 12, values.length > 2);
        if (values[2]) values[2] = checkYear(values[2]);
        var output = values.map(function (v, i) {
            return v.length == 2 && i < 2 ? v + ' / ' : v;
        });
   
        this.value = output.join('').substr(0, 14);        
    });

}

export function validateDate(val, errorId, dateValidationScheme = dateValidationSchemes.OVER_18) {
    var regexFormatString = RegExp(/^(([0-9])|([0-2][0-9])|([3][0-1]))\ (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\ \d{4}$/);
    var regex = RegExp(/^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$/);
    let stringDate = regexFormatString.test(val);
    if (!stringDate) {
        val = val.replace(/\s/g, '');
    }
    if (regex.test(val) == false && stringDate == false) {
        $('#' + errorId).show();
        $('#' + errorId).text("Please enter a valid date in the format DD / MM / YYYY");
        return false;
    } else {
        $('#' + errorId).hide();
        if (!stringDate) {
            var dateParts = val.split("/");
            var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
            if (dateValidationScheme == dateValidationSchemes.OVER_18) {
                var age = _calculateAge(dateObject);
                if (age < 18) {
                    $('#' + errorId).show();
                    $('#' + errorId).text("You must be at least 18 years old to create an account");
                    return false;
                }
            } else if (dateValidationScheme == dateValidationSchemes.FUTURE_DATES || dateValidationScheme == dateValidationSchemes.FUTURE_DATES_6M) {
                if (dateObject < new Date().setHours(0, 0, 0, 0)) {
                    $('#' + errorId).show();
                    $('#' + errorId).text("Please enter a date in the future");
                    return false;
                }
            }
            if (dateValidationScheme == dateValidationSchemes.FUTURE_DATES_6M) {
                if (dateObject > new Date().setMonth((new Date()).getMonth() + 6)) {
                    $('#' + errorId).show();
                    $('#' + errorId).text("Please enter a date in the next 6 months");
                    return false;
                }
            }
            return true;
        } else {
            return true;
        }
    }
}

function checkValue(str, max, fullNumber) {
    if (str.charAt(0) !== '0' || str == '00') {
        var num = parseInt(str);
        if (isNaN(num) || num <= 0 || num > max) num = 1;
        str = (num > parseInt(max.toString().charAt(0)) || fullNumber)
            && num.toString().length == 1 ? '0' + num : num.toString();
    };
    return str;
};

function checkYear(str) {
    const thisYear = (new Date()).getFullYear();
    const num = parseInt(str);
    if (str.length === 2 && num != 19 && num != 20) {
        if (isNaN(num) || num < 0) return str;
        // Assume 20 years in the future, or 80 in the past
        if (num < thisYear - 2000 + 20) {
            str = (2000 + num).toString();
        } else {
            str = (1900 + num).toString();
        }
    }
    return str;
}

function _calculateAge(birthday) { // birthday is a date    
    var ageDifMs = Date.now() - birthday.getTime();
    var ageDate = new Date(ageDifMs); // miliseconds from epoch
    return Math.abs(ageDate.getUTCFullYear() - 1970);
}
