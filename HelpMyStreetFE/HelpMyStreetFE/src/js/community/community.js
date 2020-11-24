import { initialiseSliders } from "../shared/image-slider.js";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";
import { hidePopup, showServerSidePopup } from "../shared/popup";


$(document).ready(function () {
    initialiseSliders();

    if ($('.select-all').length > 0) {
        $('.btn--sign-up').addClass("disabled");
    }

    $('.select-all').change(function () {
        var values = []
        $('.select-all').each(function () {
            values.push($(this).is(":checked"));
        })
        if (!values.includes(false)) {
            $('.btn--sign-up').removeClass("disabled");
        } else {
            $('.btn--sign-up').addClass("disabled");
        }
    })
});


//  Maps TODO: refactor the common code in this and coveragemap.js into a separate file

const largeAreaZoomNumber = 10;  // zoom level when min distance between volunteers is populated in call to User Service


let script = document.createElement('script');
script.src = '/api/Maps/js';
script.defer = false;
script.async = false;

document.head.appendChild(script);

let googleMap;
let googleMapMarkers = new Map();
let postcodeMarker = null;

let previousZoomLevel = -1;

window.initGoogleMap = async function () {

    let initialLat = parseFloat($("#Latitude").val());
    let initialLng = parseFloat($("#Longitude").val());
    let zoomLevel = parseFloat($("#ZoomLevel").val());

    let noPoi = [
        {
            featureType: "poi.attraction",
            stylers: [
                {
                    visibility: "off"
                }
            ]
        },
        {
            featureType: "poi.business",
            stylers: [
                {
                    visibility: "off"
                }
            ]
        },
        {
            featureType: "poi.government",
            stylers: [
                {
                    visibility: "off"
                }
            ]
        },
        {
            featureType: "poi.medical",
            stylers: [
                {
                    visibility: "off"
                }
            ]
        },
        {
            featureType: "poi.park",
            elementType: "labels.icon",
            stylers: [
                {
                    visibility: "off"
                }
            ]
        },
        {
            featureType: "poi.place_of_worship",
            stylers: [
                {
                    visibility: "off"
                }
            ]
        },
        {
            featureType: "poi.school",
            stylers: [
                {
                    visibility: "off"
                }
            ]
        },
        {
            featureType: "poi.sports_complex",
            stylers: [
                {
                    visibility: "off"
                }
            ]
        },
        {
            featureType: "transit",
            stylers: [
                {
                    visibility: "off"
                }
            ]
        }
    ];

    googleMap = new google.maps.Map(document.getElementById('map'), {
        center: { lat: initialLat, lng: initialLng },
        mapTypeControl: false,
        streetViewControl: false,
        fullscreenControl: false,
        zoom: zoomLevel,
        mapTypeId: 'roadmap',
        gestureHandling: 'none',
        zoomControl: false
    });

    googleMap.setOptions({ styles: noPoi });


    googleMap.addListener('idle', function () {
        let bounds = googleMap.getBounds();
        let ne = bounds.getNorthEast();
        let sw = bounds.getSouthWest();
        let swLat = sw.lat();
        let swLng = sw.lng();
        let neLat = ne.lat();
        let neLng = ne.lng();

        updateMap(swLat, swLng, neLat, neLng);
    });

};

function removedMarkerForPostcodeLookup() {
    if (postcodeMarker) {
        postcodeMarker.setMap(null);
        postcodeMarker = null;
    }
}


function setMapCentre(latitude, longitude, zoomLevel) {
    googleMap.setCenter({ lat: latitude, lng: longitude });
    googleMap.setZoom(zoomLevel);
}

async function updateMap(swLat, swLng, neLat, neLng) {

    let zoomLevel = googleMap.getZoom();

    let distanceInMeters = getDistanceInMeters(swLat, swLng, neLat, neLng);
    let minDistanceBetweenInMetres = 0;

    let isMapShowingLargeArea = false;
    if (zoomLevel <= largeAreaZoomNumber) {
        minDistanceBetweenInMetres = Math.ceil(distanceInMeters / 30);
        isMapShowingLargeArea = true;
    }

    let coords = await getVolunteers(swLat, swLng, neLat, neLng, minDistanceBetweenInMetres);

    if (zoomLevel <= largeAreaZoomNumber) {
        deleteMarkers();
        removedMarkerForPostcodeLookup();
    }

    // delete min distance markers when zooming in
    if (zoomLevel === (largeAreaZoomNumber + 1) && (previousZoomLevel === largeAreaZoomNumber)) {
        deleteMarkers();
        removedMarkerForPostcodeLookup();
    }

    coords.map(coord => {
        let thisMarker;
        if (isMapShowingLargeArea === true) {
            thisMarker = new google.maps.Marker({
                position: { lat: coord.lat, lng: coord.lng },
                title: null,
                icon: { url: "/img/logos/markers/hms5.png", scaledSize: new google.maps.Size(30, 30) }
            });

        } else {
            thisMarker = new google.maps.Marker({
                position: { lat: coord.lat, lng: coord.lng },
                title: coord.pc,
                icon: { url: "/img/logos/markers/hms5.png", scaledSize: new google.maps.Size(35, 35) }
            });
        }
        addMarker(thisMarker);
    });

    showMarkers();
    previousZoomLevel = zoomLevel;
}

function getDistanceInMeters(lat1, lon1, lat2, lon2) {
    let R = 6376.5;
    let dLat = deg2rad(lat2 - lat1);
    let dLon = deg2rad(lon2 - lon1);
    let a =
        Math.sin(dLat / 2) * Math.sin(dLat / 2) +
        Math.cos(deg2rad(lat1)) * Math.cos(deg2rad(lat2)) *
        Math.sin(dLon / 2) * Math.sin(dLon / 2);
    let c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    let d = R * c;
    return (d * 1000);

    function deg2rad(deg) {
        return deg * (Math.PI / 180);
    }
}

function addMarker(marker) {
    let key = getMarkerKey(marker);

    if (!googleMapMarkers.has(key)) {

        googleMapMarkers.set(key, marker);
    }
}

function getMarkerKey(marker) {
    return marker.getPosition().lat() + '_' + marker.getPosition().lng();
}

function setMapOnAll(googleMap) {
    googleMapMarkers.forEach(function (value, key, mapCollection) { value.setMap(googleMap); });
}

function clearMarkers() {
    setMapOnAll(null);
}

function showMarkers() {
    setMapOnAll(googleMap);
}

function deleteMarkers() {
    clearMarkers();
    googleMapMarkers.clear();
}

async function getVolunteers(swLat, swLng, neLat, neLng, minDistanceBetweenInMetres) {
    let endpoint = '/api/Maps/volunteerCoordinates?SWLatitude=' + swLat + '&SWLongitude=' + swLng + '&NELatitude=' + neLat + '&NELongitude=' + neLng + '&VolunteerType=3&IsVerifiedType=3&MinDistanceBetweenInMetres=' + minDistanceBetweenInMetres;
    const content = await hmsFetch(endpoint);
    if (content.fetchResponse == fetchResponses.SUCCESS) {
        var payload = await content.fetchPayload;
        return payload.volunteerCoordinates;
    } else {
        return [];
    }
}

$(document).ready(function () {
    const groupId = $('.community').data('group-id');
    if ($("#ShowRequestHelpPopup").val() == "True") {
        $('.btn--request-help').on('click', async function (event) {
            event.preventDefault();
            let popup = await showServerSidePopup('/api/community/get-request-help-community-popup?g=' + groupId, {
                acceptCallbackAsync: () => {
                    window.location.href = $(this).attr('href');
                    return true;
                },
                rejectCallbackAsync: () => {
                    showServerSidePopup('/api/community/get-request-help-elsewhere-popup?g=' + groupId, {
                        acceptCallbackAsync: () => {
                            window.location.href = '/request-help';
                            return true;
                        },
                    });
                    console.log(popup);
                    hidePopup(popup, 0);
                }
            });
        });
    }

    $('.btn--join-group').on('click', function (event) {
        event.preventDefault();
        showServerSidePopup('/api/community/get-join-group-popup?g=' + groupId, {
            acceptCallbackAsync: async () => {
                const content = await hmsFetch("/api/groups/join-group?g=" + groupId);
                if (content.fetchResponse == fetchResponses.SUCCESS) {
                    $('.show-to-members').removeClass('dnone');
                    $('.show-to-non-members').addClass('dnone');
                    return true;
                } else {
                    return false;
                }
            }
        });
    });

    $('.btn--leave-group').on('click', function (event) {
        event.preventDefault();
        showServerSidePopup('/api/community/get-leave-group-popup?g=' + groupId, {
            acceptCallbackAsync: async () => {
                const content = await hmsFetch("/api/groups/leave-group?g=" + groupId);
                if (content.fetchResponse == fetchResponses.SUCCESS) {
                    $('.show-to-members').addClass('dnone');
                    $('.show-to-non-members').removeClass('dnone');
                    return true;
                } else {
                    return false;
                }
            }
        });
    });

});