import $ from "jquery"

$(function () {
    console.log("loaded");

$(".tablinks").click(function (){
    console.log("click fired");
    var tablinks = document.getElementsByClassName("tablinks");
    var tabcontent = document.getElementsByClassName("tabcontent");

    tablinks.forEach(tab => {
        tab.className.replace(" active", "");
    })

    $(this).addClass("active");

    tabcontent.forEach(tab => {
        if (tabcontent.id == $(this).id + "-content") {
            tab.className.replace("inactive", "active");

        }
        else {
            tab.className.replace("active", "inactive");
        }
    })
})
})