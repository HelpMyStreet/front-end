﻿export { INACTIVITY_STATES, initialiseInactivityMonitor, getInactivityState };

import { hmsFetch, fetchResponses } from "./hmsFetch";

const IDLE_TIMEOUT = 1000 * 60 * 5; // 5 minutes

const INACTIVITY_STATES = {
  ACTIVE: "Activity detecteed in last 5 minutes",
  INACTIVE: "No activity detected in last 5 minutes",
};

let inactive;

async function initialiseInactivityMonitor() {
  var timer;

  window.onload = resetTimer;
  document.onmousemove = resetTimer;
  document.onkeydown = resetTimer;
  document.onmousedown = resetTimer;
  document.addEventListener('scroll', resetTimer, true);

  await resetTimer();

  function setInactiveFlag() {
    inactive = true;
  }

  async function resetTimer() {
    if (inactive && window.location.pathname.startsWith('/account/')) {
      var response = await hmsFetch('/account/GetLoggedInStatus');
      if (response.fetchResponse == fetchResponses.UNAUTHORISED) {
        window.location.replace('/account/Login?ReturnUrl=' + encodeURIComponent((window.location.pathname + window.location.search)));
      }
    }
    inactive = false;
    clearTimeout(timer);
    timer = setTimeout(setInactiveFlag, IDLE_TIMEOUT);
  }
}

function getInactivityState() {
  if (inactive == null) {
    initialiseInactivityMonitor();
  }

  return inactive ? INACTIVITY_STATES.INACTIVE : INACTIVITY_STATES.ACTIVE;
}