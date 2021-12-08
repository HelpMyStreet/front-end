//import { resolve } from "core-js/fn/promise";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";

let defaultOptions = {
  displayVolunteers: true,
  displayGroups: true,
  allowNavigation: true,
  allowSearch: true,
  consoleCoordinates: false,
  initialLat: 55.0,
  initialLng: -10.0,
  initialZoom: 5.3,
  divID: "map",
  singlePin: false,
};

export const defaultMarkerIcons = {
  vaccination: {
    url: "/img/logos/markers/vaccination-marker.svg",
    scaledSize: {x: 50, y: 70},
},
  task: {
    url: "/img/logos/markers/hms5.png",
    scaledSize: {x:50, y: 50},
}
}

var visiMaps = [];

const largeAreaZoomNumber = 10; // zoom level when min distance between volunteers is populated in call to User Service
const maxUserZoom = 16; // closest zoom for a user pressing the +
let maxGeometryZoom = 13; // closest zoom when searching for a place
let idleListener;

function getInfoWindowContents(
  linkURL,
  bannerLocation,
  friendlyName,
  groupType
) {
  return `<div class="community-marker">
    <div>
      <a href="${linkURL}">
        ${bannerLocation}
        <div class="marker-title">
          <h4>${friendlyName}</h4>
          <div class="marker-subtitle">
            <p>
              ${groupType}
              <span class="visit-homepage">Visit homepage</span>
            </p>
          </div>
        </div>
      </a>
    </div>
  </div>`;
}

let googleMapMarkers = new Map();
let postcodeMarker = null;

let previousZoomLevel = -1;

let geolocationState = {
  hover: false,
  pending: false,
  failed: false,
  active: false,

  setHover: function (value) {
    this.hover = value;
    this.updateIcon();
  },

  geoLocationInProgress: function () {
    this.pending = true;
    this.updateIcon();
  },

  geolocationComplete: function (success) {
    this.active = success;
    this.failed = !success;
    this.pending = false;
    this.hover = false;
    this.updateIcon();
  },

  setActive: function (value) {
    this.active = value;
    this.updateIcon();
  },

  updateIcon: function () {
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
    $("#your-location-image").css("background-position", offset + "px 0px");
  },
};

const noPoi = [
  {
    featureType: "poi.attraction",
    stylers: [
      {
        visibility: "off",
      },
    ],
  },
  {
    featureType: "poi.business",
    stylers: [
      {
        visibility: "off",
      },
    ],
  },
  {
    featureType: "poi.government",
    stylers: [
      {
        visibility: "off",
      },
    ],
  },
  {
    featureType: "poi.medical",
    stylers: [
      {
        visibility: "off",
      },
    ],
  },
  {
    featureType: "poi.park",
    elementType: "labels.icon",
    stylers: [
      {
        visibility: "off",
      },
    ],
  },
  {
    featureType: "poi.place_of_worship",
    stylers: [
      {
        visibility: "off",
      },
    ],
  },
  {
    featureType: "poi.school",
    stylers: [
      {
        visibility: "off",
      },
    ],
  },
  {
    featureType: "poi.sports_complex",
    stylers: [
      {
        visibility: "off",
      },
    ],
  },
  {
    featureType: "transit",
    stylers: [
      {
        visibility: "off",
      },
    ],
  },
];

export async function enableMaps() {
  if (typeof(google) == "undefined") {
    let scriptComplete = new Promise(function (resolve) {
      let script = document.createElement("script");
      script.src = "/api/Maps/js";
      script.defer = false;
      script.async = false;
      document.head.appendChild(script);
      window.initGoogleMap = () => resolve(true);
    });
    return await scriptComplete;
  } else {
    return true;
  }
}

export async function drawMap(inputOptions) {
  var options = {};
  Object.assign(options, defaultOptions);

  if (!inputOptions) {
    if (window.innerWidth <= 1000) {
      options.initialLng = -4.5;
      maxGeometryZoom = 11;
    }
  } else {
    Object.assign(options, inputOptions);
  }
  // re-center map for narrow screens/mobile

  let thisGoogleMap = new google.maps.Map(
    document.getElementById(options.divID),
    {
      center: { lat: options.initialLat, lng: Number(options.initialLng) },
      mapTypeControl: false,
      streetViewControl: false,
      fullscreenControl: false,
      zoom: options.initialZoom,
      mapTypeId: "roadmap",
      gestureHandling: options.allowNavigation ? "auto" : "none",
      zoomControl: options.allowNavigation,
    }
  );

  if (options.singlePin) {
    var markerOptions = {
      position: { lat: options.initialLat, lng: options.initialLng },
      clickable: false,
      title: null,
      map: thisGoogleMap,
    };
    if (typeof(options.singlePin) === 'object'){
      Object.assign(markerOptions, options.singlePin);
    }
    let thisMarker = new google.maps.Marker(markerOptions);
    if (options.singlePin.clickListener){
     thisMarker.addListener('click', options.singlePin.clickListener);   
    }
  }

  thisGoogleMap.setOptions({ styles: noPoi, maxZoom: maxUserZoom });

  if (options.allowSearch) {
    var autocompleteInput = document.getElementById("pac-input");
    thisGoogleMap.controls[google.maps.ControlPosition.TOP_LEFT].push(
      autocompleteInput
    );

    var autocomplete = new google.maps.places.Autocomplete(autocompleteInput);
    autocomplete.setComponentRestrictions({ country: "uk" });

    autocomplete.addListener("place_changed", function () {
      var place = autocomplete.getPlace();

      if (!place.geometry) {
        var autocompleteService = new google.maps.places.AutocompleteService();
        autocompleteService.getPlacePredictions(
          {
            input: autocompleteInput.value,
            componentRestrictions: { country: "uk" },
          },
          (result, status) => {
            var placesService = new google.maps.places.PlacesService(
              thisGoogleMap
            );
            placesService.getDetails(
              { placeId: result[0].place_id },
              (result, status) => {
                place = result;
                if (!place.geometry) {
                  console.log("No geometry for this place");
                  return;
                }
                autocompleteInput.value = place.name;
                autocompleteInput.blur();
                showGeometry(thisGoogleMap, place.geometry);
              }
            );
          }
        );
      } else {
        showGeometry(thisGoogleMap, place.geometry);
      }
    });
  }

  if (options.allowNavigation) {
    var geolocateButton = document.getElementById("your-location");
    thisGoogleMap.controls[google.maps.ControlPosition.TOP_RIGHT].push(
      geolocateButton
    );

    geolocateButton.addEventListener("click", function () {
      if (navigator.geolocation) {
        geolocationState.geoLocationInProgress();
        navigator.geolocation.getCurrentPosition(
          (position) => geoLocationSuccess(position, thisGoogleMap),
          geolocationState.geolocationComplete(false),
          { enableHighAccuracy: true }
        );
      } else {
        geolocationState.geolocationComplete(false);
      }
    });

    geolocateButton.addEventListener("mouseover", function () {
      geolocationState.setHover(true);
    });

    geolocateButton.addEventListener("mouseout", function () {
      geolocationState.setHover(false);
    });

    thisGoogleMap.addListener("dragstart", function () {
      geolocationState.setActive(false);
    });
  }

  idleListener = thisGoogleMap.addListener("idle", () => {
    let bounds = thisGoogleMap.getBounds();
    let ne = bounds.getNorthEast();
    let sw = bounds.getSouthWest();
    let swLat = sw.lat();
    let swLng = sw.lng();
    let neLat = ne.lat();
    let neLng = ne.lng();
    updateMap(thisGoogleMap, options, swLat, swLng, neLat, neLng);
    thisGoogleMap.setOptions({ maxZoom: maxUserZoom });
  });

  visiMaps.push(thisGoogleMap);
}

function removedMarkerForPostcodeLookup() {
  if (postcodeMarker) {
    postcodeMarker.setMap(null);
    postcodeMarker = null;
  }
}

function geoLocationSuccess(position, googleMap) {
  setMapCentre(
    googleMap,
    position.coords.latitude,
    position.coords.longitude,
    maxGeometryZoom
  );
  geolocationState.geolocationComplete(true);
}

function showGeometry(googleMap, geometry) {
  googleMap.setOptions({ maxZoom: maxGeometryZoom });
  if (geometry.viewport) {
    googleMap.fitBounds(geometry.viewport);
  } else {
    googleMap.setCenter(geometry.location);
    googleMap.setZoom(maxGeometryZoom);
  }
  geolocationState.setActive(false);
}

function setMapCentre(googleMap, latitude, longitude, zoomLevel) {
  googleMap.setCenter({ lat: latitude, lng: longitude });
  googleMap.setZoom(zoomLevel);
}

async function updateMap(googleMap, options, swLat, swLng, neLat, neLng) {
  let zoomLevel = googleMap.getZoom();

  let northSouthDistanceInMeters = getDistanceInMeters(
    swLat,
    swLng,
    neLat,
    swLng
  );
  let minDistanceBetweenInMetres = 0;

  let isMapShowingLargeArea = false;
  if (zoomLevel <= largeAreaZoomNumber) {
    minDistanceBetweenInMetres = Math.ceil(northSouthDistanceInMeters / 30);
    isMapShowingLargeArea = true;
  }

  if (zoomLevel <= largeAreaZoomNumber) {
    clearMarkers(googleMap);
    removedMarkerForPostcodeLookup(googleMap);
  }

  // delete min distance markers when zooming in
  if (
    zoomLevel === largeAreaZoomNumber + 1 &&
    previousZoomLevel === largeAreaZoomNumber
  ) {
    clearMarkers();
    removedMarkerForPostcodeLookup(googleMap);
  }

  if (options.displayVolunteers) {
    let coords = await getVolunteers(
      swLat,
      swLng,
      neLat,
      neLng,
      minDistanceBetweenInMetres
    );

    coords.map((coord) => {
      let thisMarker;
      if (isMapShowingLargeArea === true) {
        thisMarker = new google.maps.Marker({
          position: { lat: coord.lat, lng: coord.lng },
          clickable: false,
          title: null,
          icon: {
            url: "/img/logos/markers/hms5.png",
            scaledSize: new google.maps.Size(30, 30),
          },
        });
      } else {
        thisMarker = new google.maps.Marker({
          position: { lat: coord.lat, lng: coord.lng },
          clickable: false,
          title: coord.pc,
          icon: {
            url: "/img/logos/markers/hms5.png",
            scaledSize: new google.maps.Size(35, 35),
          },
        });
      }
      addMarker(thisMarker);
    });
  }

  if (options.displayGroups) {
    let communityMarkerCoords = await getCommunities();

    var infoWindows = [];

    communityMarkerCoords.map((coord) => {
      if (
        (zoomLevel >= coord.zoomLevel || zoomLevel > 10) &&
        coord.displayOnMap &&
        swLng <= coord.longitude &&
        coord.longitude <= neLng &&
        swLat <= coord.latitude &&
        coord.latitude <= neLat
      ) {
        //Map zooms for homepages don't correlate well with when you'd want to "see" the blue pin

        let thisMarker;
        let thisInfoWindow;
        let thisBannerLocation = "";
        if (coord.bannerLocation != "") {
          thisBannerLocation = `<img src="${coord.bannerLocation}"></img>`;
        }

        thisInfoWindow = new google.maps.InfoWindow({
          content: getInfoWindowContents(
            coord.linkURL,
            thisBannerLocation,
            coord.friendlyName,
            coord.groupType
          ),
        });
        thisMarker = new google.maps.Marker({
          zoomLevel: zoomLevel,
          type: "community",
          position: { lat: coord.latitude, lng: coord.longitude },
          title: coord.friendlyName,
          icon: {
            url: "/img/logos/markers/hms2.png",
            scaledSize: new google.maps.Size(70, 70),
          },
          zIndex: 1000,
          animation: google.maps.Animation.BOUNCE,
        });
        infoWindows.push({ marker: thisMarker, infoWindow: thisInfoWindow });
        thisMarker.addListener("click", () => {
          google.maps.event.removeListener(idleListener);
          thisInfoWindow.open(googleMap, thisMarker);
          thisMarker.setAnimation(null);
          setTimeout(() => {
            idleListener = googleMap.addListener("idle", () => {
              let bounds = googleMap.getBounds();
              let ne = bounds.getNorthEast();
              let sw = bounds.getSouthWest();
              let swLat = sw.lat();
              let swLng = sw.lng();
              let neLat = ne.lat();
              let neLng = ne.lng();
              updateMap(googleMap, options, swLat, swLng, neLat, neLng);
              googleMap.setOptions({ maxZoom: maxUserZoom });
            });
          }, 500);
        });
        setTimeout(() => thisMarker.setAnimation(null), 2000);
        addMarker(thisMarker);
      }
    });
  }

  showMarkers(googleMap);

  previousZoomLevel = zoomLevel;

  if (options.consoleCoordinations) {
    console.log(
      `coords: ${googleMap.getCenter()} zoom: ${googleMap.getZoom()}`
    );
  }
}

function getDistanceInMeters(lat1, lon1, lat2, lon2) {
  let R = 6376.5;
  let dLat = deg2rad(lat2 - lat1);
  let dLon = deg2rad(lon2 - lon1);
  let a =
    Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(deg2rad(lat1)) *
      Math.cos(deg2rad(lat2)) *
      Math.sin(dLon / 2) *
      Math.sin(dLon / 2);
  let c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
  let d = R * c;
  return d * 1000;

  function deg2rad(deg) {
    return deg * (Math.PI / 180);
  }
}

function addMarker(marker) {
  let key = getMarkerKey(marker);
  let alreadyExists = googleMapMarkers.has(key);
  if (!alreadyExists) {
    googleMapMarkers.set(key, marker);
  }
}

function getMarkerKey(marker) {
  if (marker.type == "community") {
    return `community_${marker.title}`;
  } else {
    return marker.getPosition().lat() + "_" + marker.getPosition().lng();
  }
}

function setMapOnAll(googleMap) {
  googleMapMarkers.forEach(function (value, key, mapCollection) {
    var onMap = value.getMap() != undefined && value.getMap() == googleMap;
    if (!onMap) {
      value.setMap(googleMap);
    }
  });
}

function clearMarkers(googleMap) {
  googleMapMarkers.forEach(function (value, key, mapCollection) {
    if (value.getMap() == googleMap) {
      if (value.type == "community") {
        if (googleMap.getZoom() < value.zoomLevel) {
          value.setMap(null);
          googleMapMarkers.delete(key);
        }
      } else {
        value.setMap(null);
        googleMapMarkers.delete(key);
      }
    }
  });
}

function showMarkers(googleMap) {
  setMapOnAll(googleMap);
}

async function getVolunteers(
  swLat,
  swLng,
  neLat,
  neLng,
  minDistanceBetweenInMetres
) {
  let endpoint =
    "/api/Maps/volunteerCoordinates?SWLatitude=" +
    swLat +
    "&SWLongitude=" +
    swLng +
    "&NELatitude=" +
    neLat +
    "&NELongitude=" +
    neLng +
    "&VolunteerType=3&IsVerifiedType=3&MinDistanceBetweenInMetres=" +
    minDistanceBetweenInMetres;
  const content = await hmsFetch(endpoint);
  if (content.fetchResponse == fetchResponses.SUCCESS) {
    var payload = await content.fetchPayload;
    if (payload.volunteerCoordinates != null) {
      return payload.volunteerCoordinates;
    } else {
      return [];
    }
  } else {
    return [];
  }
}

async function getCommunities() {
  let endpoint = "/api/Maps/getCommunities";
  const content = await hmsFetch(endpoint);
  if (content.fetchResponse == fetchResponses.SUCCESS) {
    let thisPayload = await content.fetchPayload;
    return thisPayload.communityDetails;
  } else {
    return [];
  }
}
