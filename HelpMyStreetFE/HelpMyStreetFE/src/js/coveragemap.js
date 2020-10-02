import { hmsFetch, fetchResponses } from "./shared/hmsFetch.js";

const largeAreaZoomNumber = 10;  // zoom level when min distance between volunteers is populated in call to User Service
const closeUpZoomNumber = 16; // zoom level when postcode is entered
let initialUKZoomNumber = 5.3; // zoom level of the UK when geo location is not enabled
const geolocationZoomNumber = 14; // zoom level when geo location is enabled

let initialLat = 55.0;
let initialLng = -10.0;

let script = document.createElement('script');
script.src = '/api/Maps/js';
script.defer = false;
script.async = false;

document.head.appendChild(script);

let googleMap;
let googleMapMarkers = new Map();
let postcodeMarker = null;

let previousZoomLevel = -1;

let geolocationState = {
    'hover': false,
    'pending': false,
    'failed': false,
    'active': false,

    'setHover': function (value) {
        this.hover = value;
        this.updateIcon();
    },

    'geoLocationInProgress': function () {
        this.pending = true;
        this.updateIcon();
    },

    'geolocationComplete': function (success) {
        this.active = success;
        this.failed = !success;
        this.pending = false;
        this.hover = false;
        this.updateIcon();
    },

    'setActive': function (value) {
        this.active = value;
        this.updateIcon();
    },

    'updateIcon': function () {
        let icon = 0;
        if (this.pending === true) {
            icon = 3;
        } else if (this.failed === true) {
            icon = 1;
        } else if (this.active === true) {
            icon = 8;
        } else if (this.hover === true) {
            icon = 2;
        }
        let offset = icon * -24;
        $('#your-location-image').css('background-position', offset + 'px 0px');
    }
};

window.initGoogleMap = async function () {

    // re-center map for narrow screens/mobile
    if (window.innerWidth <= 1000) {
        initialLng = -4.5;
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
        mapTypeControl: false,
        streetViewControl: false,
        fullscreenControl: false,
        zoom: initialUKZoomNumber,
        mapTypeId: 'roadmap'
    });

    googleMap.setOptions({ styles: noPoi });


    var autocompleteInput = document.getElementById('pac-input');
    googleMap.controls[google.maps.ControlPosition.TOP_LEFT].push(autocompleteInput);

    var geolocateButton = document.getElementById('your-location');
    googleMap.controls[google.maps.ControlPosition.TOP_RIGHT].push(geolocateButton);

    geolocateButton.addEventListener('click', function () {
        if (navigator.geolocation) {
            geolocationState.geoLocationInProgress();
            navigator.geolocation.getCurrentPosition(geoLocationSuccess, geolocationState.geolocationComplete(false), { enableHighAccuracy: true });
        } else {
            geolocationState.geolocationComplete(false)
        }
    });

    geolocateButton.addEventListener('mouseover', function () {
        geolocationState.setHover(true);
    });

    geolocateButton.addEventListener('mouseout', function () {
        geolocationState.setHover(false);
    });

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

    googleMap.addListener('dragstart', function () {
        geolocationState.setActive(false);
    });

 
    var autocomplete = new google.maps.places.Autocomplete(autocompleteInput);
    autocomplete.setComponentRestrictions({ 'country': 'uk' });

    autocomplete.addListener('place_changed', function () {
        var place = autocomplete.getPlace();

        if (!place.geometry) {
            var autocompleteService = new google.maps.places.AutocompleteService();
            autocompleteService.getPlacePredictions({ input: autocompleteInput.value, componentRestrictions: { country: 'uk' } }, (result, status) => {
                var placesService = new google.maps.places.PlacesService(googleMap);
                placesService.getDetails({ placeId: result[0].place_id }, (result, status) => {
                    place = result;
                    if (!place.geometry) {
                        console.log("No geometry for this place");
                        return
                    }
                    autocompleteInput.value = place.name;
                    autocompleteInput.blur();
                    showGeometry(place.geometry);
                })

            });
        } else {
            showGeometry(place.geometry);
        }
    })
};

function removedMarkerForPostcodeLookup() {
    if (postcodeMarker) {
        postcodeMarker.setMap(null);
        postcodeMarker = null;
    }
}

function geoLocationSuccess(position) {
    setMapCentre(position.coords.latitude, position.coords.longitude, geolocationZoomNumber);
    geolocationState.geolocationComplete(true);
}

function showGeometry(geometry) {
    if (geometry.viewport) {
        googleMap.fitBounds(geometry.viewport);
    } else {
        googleMap.setCenter(geometry.location);
        googleMap.setZoom(closeUpZoomNumber);
    }
    geolocationState.setActive(false);
}

function setMapCentre(latitude, longitude, zoomLevel) {
    googleMap.setCenter({ lat: latitude, lng: longitude });
    googleMap.setZoom(zoomLevel);
}

async function updateMap(swLat, swLng, neLat, neLng) {

    let zoomLevel = googleMap.getZoom();

    let northSouthDistanceInMeters = getDistanceInMeters(swLat, swLng, neLat, swLng);
    let minDistanceBetweenInMetres = 0;

    let isMapShowingLargeArea = false;
    if (zoomLevel <= largeAreaZoomNumber) {
        minDistanceBetweenInMetres = Math.ceil(northSouthDistanceInMeters / 30);
        isMapShowingLargeArea = true;
    }

    let coords = await getVolunteers(swLat, swLng, neLat, neLng, minDistanceBetweenInMetres);
    let communityMarkerCoords = await getCommunities();
    console.log(coords);
    console.log(communityMarkerCoords)

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


    if (zoomLevel > 10) {
        communityMarkerCoords.map(coord => {
            let thisMarker;
            let thisInfoWindow;
            thisInfoWindow = new google.maps.InfoWindow({
                content: `<div class="community-marker"><div><img src="${coord.bannerLocation}"></img><div class="marker-title"><h4>${coord.friendlyName}</h4><p>Local Group</p></div></div><p><a href="${coord.linkURL}">Visit homepage</a></p></div>`
            });
            thisMarker = new google.maps.Marker({
                position: { lat: coord.latitude, lng: coord.longitude },
                title: coord.friendlyName,
                icon: { url: "/img/logos/markers/hms2.png", scaledSize: new google.maps.Size(40, 40) },
                zIndex: 100
            });
            thisMarker.addListener("click", () => {
                thisInfoWindow.open(googleMap, thisMarker);
            });
            addMarker(thisMarker);
        });
    }
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
    let endpoint = '/api/Maps/volunteerCoordinates?SWLatitude=' + swLat + '&SWLongitude=' + swLng + '&NELatitude=' + neLat + '&NELongitude=' + neLng + '&VolunteerType=3&IsVerifiedType=3&MinDistanceBetweenInMetres=' + minDistanceBetweenInMetres;
    const content = await hmsFetch(endpoint);
    if (content.fetchResponse == fetchResponses.SUCCESS) {
        var payload = await content.fetchPayload;
        if (payload.volunteerCoordinates != null) {
            return payload.volunteerCoordinates;
        }
        else {
            return [];
        }
    } else {
        return [];
    }
}

async function getCommunities() {
    let endpoint = '/api/Maps/getCommunities';
    const content = await hmsFetch(endpoint);
    if (content.fetchResponse == fetchResponses.SUCCESS) {
        let thisPayload = await content.fetchPayload;
        return thisPayload.communityDetails;
    } else {
        return [];
    }
}