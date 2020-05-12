
var selectedActivities = {};
var selectedTime = {};
var selectedFor = {};

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

        var activeTab = $('.progress-bar').find('.is-active');
        var currentTab = activeTab.attr("data-tab");
        var nextTab = activeTab.next().attr("data-tab");        
        _moveTab(currentTab, nextTab);
        changeProgressNext($(this));
    })
    $('.btnBack').click(function () {
        var activeTab = $('.progress-bar').find('.is-active');
        var currentTab = activeTab.attr("data-tab");
        var previousTab = activeTab.prev().attr("data-tab");
        console.log(currentTab, previousTab);
        _moveTab(currentTab, previousTab);
        changeProgressPrev($(this));
    })

    
}

var intialiseRequestTiles = function () {
    $('.tiles__tile').click(function () {
        var type = $(this).attr("data-type");
        switch (type) {
            case "activities":
                handleActivity($(this));    
                break;
            case "timeframe":
                handleTimeFrame($(this));
                break;

            case "request-for":
                handleRequestFor($(this));
                break;

        }
    })
}


var handleRequestFor = function (el) {
    $('*[data-type="request-for"]').removeClass("selected");
    el.addClass("selected");
    selectedFor = el.attr("id");    
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


function _moveTab(currentTab, nextTab) {    
    $('*[data-tab-page="' + currentTab +'"]').hide();
    $('[data-tab-page="' + nextTab + '"]').show();
}
