import "isomorphic-fetch"
import "../sass/main.scss";
import firebase from "./firebase/index";
import account from "./account";
import notification from "./account/notification";
import "./shared/info-popup";
import "./shared/site-header";
import { intialiseCookieConsent } from "./shared/cookie-helper"
import { intialiseForgottonForm } from "./home/forgotton-password"
import { buttonLoad, buttonUnload } from "./shared/btn";
import * as MarkerClusterer from "@google/markerclustererplus";

const magicZoomNumber = 17;
const mapsAPIKey = "AIzaSyAiCC5pku-80g3yX8vVbRlXA0liX9wuX7s";

var script = document.createElement('script');
script.src = `https://maps.googleapis.com/maps/api/js?key=${mapsAPIKey}&callback=initMap`;
script.defer = true;
script.async = true;

var map;

var hmsMarkers = [];
var otherMarkers = [];

function locationSuccess(position) {
    const latitude = position.coords.latitude;
    const longitude = position.coords.longitude;
    map.setCenter({ lat: latitude, lng: longitude });
    
}

function locationError(error) {
    console.log(error);
}

function drawMap(coordinates) {
    console.log(coordinates);
    map = new google.maps.Map(document.getElementById('map'), {
        center: coordinates,
        zoom: magicZoomNumber
    });
    map.addListener('zoom_changed', manageEmptyMarkers);
}

async function getUsers() {
        const result = await fetch('/api/postcode/getAllHelpers');
        const users = await result.json();
    return users;
}

async function getPostcodes() {
    const northEast = map.getBounds().getNorthEast().toJSON();
    const southWest = map.getBounds().getSouthWest().toJSON();
    const currentLatMin = southWest.lat;
    const currentLatMax = northEast.lat;
    const currentLngMin = southWest.lng;
    const currentLngMax = northEast.lng;
    const mapCentre = map.getCenter().toJSON();
    //result = await fetch(`/api/postcode/getAllPostcodesBetween/?latMin=${currentLatMin}&latMax${currentLatMax}&lngMin=${currentLngMin}&lngMax=${currentLngMax}`);
    const lookupString = `https://api.postcodes.io/postcodes?lon=${mapCentre.lng}&lat=${mapCentre.lat}&radius=1990&limit=100`
    console.log(lookupString);
    const result = await fetch(lookupString);
    const postcodes = await result.json();
    return postcodes.result;
}

function addMarkers(users) {
    if (users.hasContent) {
        users.map(user => {
            var thisMarker = new google.maps.Marker({
                position: { lat: user.latitude, lng: user.longitude },
                title: user.postcode,
                icon: { url: "./img/logos/markers/hms5.png", scaledSize: new google.maps.Size(40, 40) }
            });
            buildInfoWindow(thisMarker, user.postcode, user.champions, user.volunteers)
            hmsMarkers.push(thisMarker);
        });
        markerCluster = new MarkerClusterer(map, hmsMarkers, { imagePath: "./img/logos/markers/hms" });
    }
    manageEmptyMarkers();

}

function buildInfoWindow(marker, postcode, champions, volunteers) {
    if (champions === 0 && volunteers === 0) {
        var usersString = '<p><center>No HMS helpers signed up yet!<center></p><p><a href="/registration/stepone">Be The First</a></p>'
    } else {
        var usersString = '<p><center>Champions: ' + champions + ' Volunteers: ' + volunteers + '<center></p>'
    }
    var contentString = '<div id="content">' +
        '<div id="siteNotice">' +
        '</div>' +
        '<div id="bodyContent">' +
        '<p><center><b>' + postcode + '</b><center></p>' +
        usersString +
        '</div>' +
        '</div>';
    var infowindow = new google.maps.InfoWindow({
        content: contentString
    });
    marker.addListener('mouseover', function () {
        infowindow.open(map, marker);
    });
    marker.addListener('mouseout', function () {
        setTimeout(infowindow.close, 3000);
    });
}

function manageEmptyMarkers() {
    if (map.getZoom() >= magicZoomNumber) {
        getPostcodes().then(postcodes => {
            postcodes.map(postcode => {
                var thisMarker = new google.maps.Marker({
                    position: { lat: postcode.latitude, lng: postcode.longitude },
                    map: map,
                    title: postcode.postcode,
                    icon: { url: "./img/logos/markers/hms1.png", scaledSize: new google.maps.Size(40, 40) }
                });
                buildInfoWindow(thisMarker, postcode.postcode, 0, 0);
                otherMarkers.push(thisMarker);
            });
        });
    } else {
        otherMarkers.map(marker => marker.setMap(null));
    }
}



window.initMap = function () {
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: -1.158109, lng: 52.954784 },
        zoom: magicZoomNumber
    });
    map.addListener('zoom_changed', manageEmptyMarkers);

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(locationSuccess, locationError);
    }
    getUsers().then(users => addMarkers(users)).catch(error => console.log(error));

};

document.head.appendChild(script);

$(function () {
    $('.no-fouc').removeClass('no-fouc');
    if (typeof configuration !== 'undefined') {
        firebase.init(JSON.parse(configuration.firebase));
    }
    window.account = account;

    intialiseCookieConsent();
    intialiseForgottonForm(firebase, account);

  $("#postcode_button").click(function(evt) {
    const postCode = $("#postcode").val();

    if (postCode) {
      $(this).width($(this).width());
      $(this).height($(this).height());

      $("#postcode_notcovered").addClass('dnone');
      $("#postcode_covered").addClass('dnone');
      $("#postcode_error").hide();
      $("#postcode_invalid").hide();
      //$("#request_help").hide();        
      $(".postcode__info").addClass('dnone');
      $('#postcode_button').removeClass('postcode_button_clicked')

        buttonLoad($(this));

        fetch(`/api/postcode/checkCoverage/${postCode}`)
        .then(resp => resp.json())
            .then(data => {
                $('#postcode_button').addClass('postcode_button_clicked')                
                var postCodeValid = (data.postCodeResponse.isSuccessful && data.postCodeResponse.hasContent);                
                if (postCodeValid == false) {
                    $(".postcode__info, #postcode_invalid").show();
                } else {
                    if (data.volunteerCount == 0 && data.championCount == 0)
                    {
                        $(".postcode__info, #postcode_notcovered").removeClass('dnone');
                    } else if (data.volunteerCount > 0 || data.championCount > 0) {
                        $(".postcode__info, #postcode_covered").removeClass('dnone');
                        //if (data.volunteerCount > 0 && data.championCount > 0) { // phase.1.1
                        //    $(".postcode__info, #request_help").show();                           
                        //}
                    }                        
                }              
        })
        .catch(err => {
          $("#postcode_error").show();
        })
        .finally(() => {
           $(this).width(null);
            $(this).height(null);
            buttonUnload($(this));
        });
    }
  });

    $("#login-submit").click(async () => {        
        buttonLoad($(this));
    try {
        $("#login-submit")[0].disabled = true;
        const email = $("#email").val();
        const password = $("#password").val();
        const response = await account.login.login(email, password);
        if (!response.success) {
        $("#login-fail-message").text(response.message);
        $("#login-submit")[0].disabled = false;
      }
    } finally {
        buttonUnload($(this));
        $("#login-submit")[0].disabled = false;
        }
    });
});
