export function datepickerLoad(id) {
    let datepicker = document.getElementById(id);


    datepicker.addEventListener("focusout", function (e) {
        validateDob(this.value, id);
    });

     // event listner to add slashes whilst entering input
    datepicker.addEventListener('input', function (e) {        
        this.type = 'text';
        var input = this.value;
        if (/\D\/$/.test(input)) input = input.substr(0, input.length - 3);
        var values = input.split('/').map(function (v) {
            return v.replace(/\D/g, '')
        });
        if (values[0]) values[0] = checkValue(values[0], 31);
        if (values[1]) values[1] = checkValue(values[1], 12);
        var output = values.map(function (v, i) {
            return v.length == 2 && i < 2 ? v + ' / ' : v;
        });
   
        this.value = output.join('').substr(0, 14);        
    });

}

export function validateDob(val, id) {
    val = val.replace(/\s/g, '');
    var regexFormatString = RegExp(/^(([0-9])|([0-2][0-9])|([3][0-1]))\ (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\ \d{4}$/);
    var regex = RegExp(/^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$/);
    let stringDate = regexFormatString.test(val);
    if (regex.test(val) == false && stringDate == false ) {
        $('#' + id).find("~ .error").show();
        $('#' + id).find("~ .error").text("Please enter a valid date of birth in the following format DD/MM/YYYY");
    } else {       
        $('#' + id).find("~ .error").hide();
        if (!stringDate) {
            var dateParts = val.split("/");
            var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);
            var age = _calculateAge(dateObject);

            if (age < 18) {
                $('#' + id).find("~ .error").show();
                $('#' + id).find("~ .error").text("You must be at least 18 years old to create an account");

            } else {
                $('#' + id).val(moment(dateObject).format('DD MMM YYYY'));
            }
        }
    }        
}

function checkValue(str, max) {
    if (str.charAt(0) !== '0' || str == '00') {
        var num = parseInt(str);
        if (isNaN(num) || num <= 0 || num > max) num = 1;
        str = num > parseInt(max.toString().charAt(0))
            && num.toString().length == 1 ? '0' + num : num.toString();
    };
    return str;
};

function _calculateAge(birthday) { // birthday is a date    
    var ageDifMs = Date.now() - birthday.getTime();
    var ageDate = new Date(ageDifMs); // miliseconds from epoch
    return Math.abs(ageDate.getUTCFullYear() - 1970);
}