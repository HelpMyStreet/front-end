export { INACTIVITY_STATES, initialiseInactivityMonitor, getInactivityState };

import { hmsFetch, fetchResponses } from "./hmsFetch";

const IDLE_TIMEOUT = 1000 * 60 * 1; // 1 minute

const INACTIVITY_STATES = {
  ACTIVE: "Activity detecteed in last 5 minutes",
  INACTIVE: "No activity detected in last 5 minutes",
};

let inactive;
let resetting;

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
        if (!resetting) {
            resetting = true;
            if (inactive && window.location.pathname.startsWith('/account/')) {
                var response = await hmsFetch('/account/get-logged-in-status');
                if (response.fetchResponse == fetchResponses.UNAUTHORISED) {
                    window.location.replace('/login?ReturnUrl=' + encodeURIComponent((window.location.pathname + window.location.search)));
                }
            }
            resetting = false;
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
