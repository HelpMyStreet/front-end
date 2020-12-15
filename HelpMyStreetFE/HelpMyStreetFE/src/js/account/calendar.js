import { Calendar } from '@fullcalendar/core';
import { hmsFetch, fetchResponses } from "../shared/hmsFetch";
import { initialiseRequests } from "../profile/requests.js";
import interactionPlugin from '@fullcalendar/interaction';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import listPlugin from '@fullcalendar/list';

async function setJobStatus(job, newStatus, targetUser) {
    const jobId = job.attr("id");
    const role = $(job).data("role");

    return await hmsFetch('/api/request-help/set-job-status?j=' + jobId + '&s=' + newStatus + '&u=' + targetUser + '&r=' + role);
}

var calendarData;
let calendar;
var jobStatus = 1;

async function getCalendarData() {
    var calendarDataFetch = await hmsFetch("/account/calendar-data");
    if (calendarDataFetch.fetchResponse == fetchResponses.SUCCESS) {
        var content = await calendarDataFetch.fetchPayload;
        calendarData = content.map(data => {
            var color;
            if (data.jobStatus == 1) {
                color = "blue";
            } else {
                color = "red";
            }
            return { id: data.jobID, title: data.postCode, start: data.dateRequested, color: color }
        });
    }
}

function updateEvents() {
    calendar.refetchEvents();
}

document.addEventListener('DOMContentLoaded', async function () {
    initialiseRequests();
    var calendarEl = document.getElementById('calendar');
    await getCalendarData();



    calendar = new Calendar(calendarEl, {
        plugins: [interactionPlugin, dayGridPlugin, timeGridPlugin, listPlugin],
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
        },
        initialDate: '2020-11-01',
        navLinks: true, // can click day/week names to navigate views
        editable: true,
        eventDisplay: 'block',
        dayMaxEvents: true, // allow "more" link when too many events
        eventClick: async function (thisEvent) {
            var eventData = await hmsFetch("/api/request-help/get-job-item?j=" + btoa(thisEvent.event.id));
            var jobDetailDiv = document.getElementById('job-detail-div');
            var divContent = await eventData.fetchPayload;
            document.getElementById('inner-job-details').innerHTML = divContent;
            jobDetailDiv.classList.add('visible');
            jobDetailDiv.classList.add('card');

            $('#close-job-detail').on('click', function () {
                jobDetailDiv.classList.remove('visible');
                jobDetailDiv.classList.remove('card');
            });

            $('.job-detail-div').on('click', '.job button.trigger-status-update-popup', async function () {
                const btn = $(this);
                const job = btn.closest(".job");
                const targetState = $(btn).data("target-state");
                const targetUser = $(btn).data("target-user") ?? "self";

                let jobId = job.attr("id");
                const role = $(job).data("role");
                let response = await setJobStatus(job, targetState, targetUser);
                if (response.fetchResponse == fetchResponses.SUCCESS) {
                    const payload = await response.fetchPayload;
                    $(job).find('.job__status__new').html(payload.newStatus);
                    $(job).find('button').toggle();
                    $(job).find('.toggle-on-status-change').toggle();
                    setTimeout(updateEvents, 1000);
                    
                }
                else {
                    console.log(response);
                }
            });
        },
        events: '/account/calendar-data',
        eventSourceSuccess: function (content, xhr) {
            console.log(content);
            if (jobStatus == 0) {
                return content;
            } else {
                return content.filter(x => x.jobStatus == jobStatus);
            }
        }
    });
    $('#filter-events').on('click', function () {
        if (jobStatus == 1) {
            jobStatus = 0;
            $('#filter-events').html('View Accepted')
        } else if (jobStatus == 0) {
            jobStatus = 2;
            $('#filter-events').html('View Completed')
        } else if (jobStatus == 2) {
            jobStatus = 3;
            $('#filter-events').html('View Open')
        }
        else {
            jobStatus = 1;
            $('#filter-events').html('View All')
        }
        calendar.refetchEvents();
    });

    calendar.render();
});
