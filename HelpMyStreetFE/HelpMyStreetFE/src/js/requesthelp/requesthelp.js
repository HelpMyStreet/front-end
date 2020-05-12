
var selectedActivities = {};
var selectedTime = {};
$(() => {
    intialiseRequestTiles();    
    initaliseProgressButtons()

});


var initaliseProgressButtons = function () {


    var changeProgressNext = function (btn) {
        var activeTab = $('.progress-bar').find('.is-active');
        activeTab.removeClass("is-active");
        activeTab.addClass("is-complete");
        var nextTab = activeTab.next();
        nextTab.addClass("is-active");
        if (nextTab.next().length == 0) {
            btn.hide();
        }
        $('.btnBack').show();
    }
    var changeProgressPrev = function (btn) {
        var activeTab = $('.progress-bar').find('.is-active');
        activeTab.removeClass("is-active");
        var prevTab = activeTab.prev();
        prevTab.removeClass("is-complete");
        prevTab.addClass("is-active");
        if (prevTab.prev().length == 0) {
            btn.hide();
        }
        $('.btnNext').show();
    }
    $('.btnNext').click(function () {
        changeProgressNext($(this));
    })
    $('.btnBack').click(function () {
        changeProgressPrev($(this));
    })

    
}

var intialiseRequestTiles = function () {
    $('.tiles__tile').click(function () {
        var id = String($(this).attr("id"));
        var type = $(this).attr("data-type");
        switch (type) {
            case "activities":
                handleActivity($(this));    
                break;
            case "timeframe":
                handleTimeFrame($(this));
                break;
        }
    })
}


var handleTimeFrame = function (el) {
    $('*[data-type="timeframe"]').removeClass("selected");
    var allowCustomEntry = el.attr("data-allowcustom");
    var selectedValue = el.attr("data-val");
    selectedTime = selectedValue
    if (allowCustomEntry == "True") {
        $("#CustomTime").show();
        $("#CustomTime").find("select").change(function () {
            selectedTime = el.children("option:selected").val();
            console.log(selectedTime);
        });
    } else {
        $("#CustomTime").hide();
    }
    el.addClass("selected");   
}

var handleActivity = function (el) {
    $('*[data-type="activities"]').removeClass("selected");
    el.addClass("selected");
    selectedActivities = el.attr("id");    
}