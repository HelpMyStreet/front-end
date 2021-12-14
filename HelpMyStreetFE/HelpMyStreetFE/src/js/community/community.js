import { initialiseSliders } from "../shared/image-slider.js";
import { hmsFetch, fetchResponses } from "../shared/hmsFetch.js";
import { hidePopup, showServerSidePopup } from "../shared/popup";
import { enableMaps, drawMap } from "../shared/maps";
import { initialiseNewsTicker } from "../components/news-ticker"

async function initialiseMaps(){
    var options = {
        displayVolunteers: true,
        displayGroups: false,
        allowNavigation: false,
        allowSearch: false,
        consoleCoordinates: false,
        initialLat: parseFloat($('#map').data('latitude')),
        initialLng: parseFloat($('#map').data('longitude')),
        initialZoom: parseFloat($('#map').data('zoom')),
        divID: "map",
        singlePin: false
    };
    if ($(`#map`).length != 0){
        drawMap(options);
        }
}

$(document).ready(function () {
    initialiseSliders();
    initialiseNewsTicker();
    enableMaps().then(() => initialiseMaps());
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

    const groupId = $('.community').data('group-id');
    $('.show-request-help-popup').on('click', async function (event) {
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
                hidePopup(popup, 0);
            }
        });
    });

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

    $('.btn--sign-up-popup').on('click', function (event) {
        event.preventDefault();
        showServerSidePopup('/api/community/get-sign-up-popup?g=' + groupId, {
            acceptCallbackAsync: () => {
                window.location.href = `/login/${groupId}?ReturnUrl=/community/joinandgo/${groupId}`;
                return true;
            },
            rejectCallbackAsync: () => {
                window.location.href = '/login/'
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