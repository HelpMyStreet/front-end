import { Calendar } from '@fullcalendar/core';
import { hmsFetch, fetchResponses } from "../shared/hmsFetch";
import interactionPlugin from '@fullcalendar/interaction';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import listPlugin from '@fullcalendar/list';

document.addEventListener('DOMContentLoaded', async function () {
    var calendarEl = document.getElementById('calendar');

    var calendarDataFetch = await hmsFetch("/account/calendar-data");
    var calendarData;
    console.log(calendarDataFetch);
    if (calendarDataFetch.fetchResponse == fetchResponses.SUCCESS) {
        var content = await calendarDataFetch.fetchPayload;
        calendarData = content;

    }

    calendarData = calendarData.map(data => {
        var color;
        if (data.jobStatus == 1) {
            color = "blue";
        } else {
            color = "red";
        }
        return { id: data.jobID, title: data.postCode, start: data.dateRequested, backgroundColor: color }
    });


    var calendar = new Calendar(calendarEl, {
        plugins: [interactionPlugin, dayGridPlugin, timeGridPlugin, listPlugin],
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },
        initialDate: '2020-11-01',
        navLinks: true, // can click day/week names to navigate views
        editable: true,
        dayMaxEvents: true, // allow "more" link when too many events
        eventClick: async function (thisEvent) {
            console.log(thisEvent);
            var eventData = await hmsFetch("/api/request-help/get-job-item?j=" + btoa(thisEvent.event.id));
            var jobDetailDiv = document.getElementById('job-detail-div');
            var divContent = await eventData.fetchPayload;
            jobDetailDiv.innerHTML = divContent;
            jobDetailDiv.classList.add('visible');
            jobDetailDiv.classList.add('card');
        },
        events: calendarData
    });

    calendar.render();
});
