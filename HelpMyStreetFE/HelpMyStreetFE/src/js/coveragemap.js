import "isomorphic-fetch";

const largeAreaZoomNumber = 10;  // zoom level when min distance between volunteers is populated in call to User Service
const closeUpZoomNumber = 16; // zoom level when postcode is entered
let initialUKZoomNumber = 6; // zoom level of the UK when geo location is not enabled
const geolocationZoomNumber = 14; // zoom level when geo location is enabled

let initialLat = 54.383618;
let initialLng = -3.821280;

let script = document.createElement('script');
script.src = 'api/Maps/js';
script.defer = false;
script.async = false;

document.head.appendChild(script);

let googleMap;
let googleMapMarkers = new Map();
let postcodeMarker = null;

let previousZoomLevel = -1;

window.initGoogleMap = async function () {

    // zoom out one step if on a mobile and change centre of map
    if (/Mobi|Android/i.test(navigator.userAgent)) {
        initialUKZoomNumber -= 1;
        initialLat = 55.841264;
        initialLng = -4.119149;
    }

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
        zoom: initialUKZoomNumber
    });

    googleMap.setOptions({ styles: noPoi });


    //if (navigator.geolocation) {
    //    navigator.geolocation.getCurrentPosition(geoLocationSuccess, (error) => { }, { enableHighAccuracy: true });
    //}

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

    //$("#postcode_button").click(async function (evt) {
    //    let postcode = $("#postcode").val();
    //    if (postcode) {
    //        let postcodeCoordinates = await getPostcodeCoordinates(postcode);
    //        if (postcodeCoordinates.isSuccessful && postcodeCoordinates.content.postcodeCoordinates.length > 0) {
    //            let postcodeCoordinate = postcodeCoordinates.content.postcodeCoordinates[0];
    //            setMapCentre(postcodeCoordinate.latitude, postcodeCoordinate.longitude, closeUpZoomNumber);

    //            postcodeMarker = new google.maps.Marker({
    //                position: { lat: postcodeCoordinate.latitude, lng: postcodeCoordinate.longitude },
    //                title: postcodeCoordinate.postcode,
    //            });

    //            addMarkerForPostcodeLookup();

    //        }
    //    }
    //});
};

function addMarkerForPostcodeLookup() {
    postcodeMarker.setMap(googleMap);
}

function removedMarkerForPostcodeLookup() {
    if (postcodeMarker) {
        postcodeMarker.setMap(null);
        postcodeMarker = null;
    }
}

function geoLocationSuccess(position) {
    setMapCentre(position.coords.latitude, position.coords.longitude, geolocationZoomNumber);
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
                icon: { url: "./img/logos/markers/hms5.png", scaledSize: new google.maps.Size(30, 30) }
            });

        } else {
            thisMarker = new google.maps.Marker({
                position: { lat: coord.lat, lng: coord.lng },
                title: coord.pc,
                icon: { url: "./img/logos/markers/hms5.png", scaledSize: new google.maps.Size(35, 35) }
            });
        }
        addMarker(thisMarker);
    });
    console.log(googleMap.getCenter().toJSON());
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

        marker.addListener('click', function () {
            setMapCentre(marker.getPosition().lat(), marker.getPosition().lng(), googleMap.getZoom() + 1);
        });

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
    let endpoint = 'api/Maps/volunteerCoordinates?SWLatitude=' + swLat + '&SWLongitude=' + swLng + '&NELatitude=' + neLat + '&NELongitude=' + neLng + '&VolunteerType=3&IsVerifiedType=3&MinDistanceBetweenInMetres=' + minDistanceBetweenInMetres;
    const content = await fetch(endpoint);
    const users = await content.json();
    return users.volunteerCoordinates;
}

async function getPostcodeCoordinates(postcode) {
    let endpoint = 'api/Maps/postcodeCoordinate?postcode=' + postcode;
    const content = await fetch(endpoint);
    const coordinates = await content.json();
    return coordinates;
}