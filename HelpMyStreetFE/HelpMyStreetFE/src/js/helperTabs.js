import $ from "jquery"

$(function () {
    $("#sign-up-helper").click(function (){
        switcheroo('sign-up-helper');
    })


    $("#request-help-helper").click(function () {
        switcheroo('request-help-helper');
    })

    function switcheroo(elementID) {
        var tablinks = document.getElementsByClassName("tablinks");
        var tabcontent = document.getElementsByClassName("tabcontent");

        tablinks.forEach(tab => {
            $("#" + tab.id).removeClass("active");
        })

        $("#"+elementID).addClass("active");

        tabcontent.forEach(tab => {
            if (tab.id == elementID + "-content") {
                $("#" + tab.id).removeClass("inactive");
                $("#" + tab.id).addClass("active");
            }
            else {
                $("#" + tab.id).removeClass("active");
                $("#" + tab.id).addClass("inactive");
            }
        })
    }
})