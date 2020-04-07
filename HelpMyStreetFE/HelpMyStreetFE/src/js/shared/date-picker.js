export function datepickerLoad(id) {
    let datepicker = document.getElementById(id);
    var maxDate = new Date();
    
    maxDate.setFullYear(maxDate.getFullYear() - 18, maxDate.getMonth(), maxDate.getDate());
    var picker = new Pikaday(
        {
            field: datepicker,
            format: "DD/MM/YYYY",
            minDate: new Date(1900, 1, 1),
            maxDate: maxDate,
            defaultDate: maxDate,
            toString(date, format) { // using moment
                return moment(date).format('DD MMM YYYY');
            },
        });

     // event listner to add slashes whilst entering input
    datepicker.addEventListener('input', function (e) {
        this.type = 'text';
        var input = this.value;
        if (/\D\/$/.test(input)) input = input.substr(0, input.length - 3);
        var values = input.split('/').map(function (v) {
            return v.replace(/\D/g, '')
        });
        if (values[0]) values[0] = checkValue(values[0], 12);
        if (values[1]) values[1] = checkValue(values[1], 31);
        var output = values.map(function (v, i) {
            return v.length == 2 && i < 2 ? v + ' / ' : v;
        });
        this.value = output.join('').substr(0, 14);
    });

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
