export const inputTypes = {
    NUMERIC: "Permit only digits",
}

export function filterInput(input, type) {
    if (type == inputTypes.NUMERIC) {
        inputFilter(input, function (value) {
            return /^\d*$/.test(value);    // Allow digits only
        }, function (value) {
            return value.replace(/\D/g, '');
        });
    }
}

function inputFilter(input, filterFn, replaceFn) {
    $(input).each(function () {
        $(this).on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
            if (filterFn(this.value)) {
                this.oldSelectionStart = this.selectionStart;
            } else {
                this.value = replaceFn(this.value);
                if (this.hasOwnProperty("oldSelectionStart")) {
                    this.setSelectionRange(this.oldSelectionStart, this.oldSelectionStart);
                }
            }
        });
    });
}
