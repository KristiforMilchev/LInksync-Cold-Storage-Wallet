! function($) {
    "use strict";

    var CalendarApp = function() {
        this.$body = $("body")
        this.$calendar = $('#calendar'),
            this.$event = ('#calendar-events div.calendar-events'),
            this.$categoryForm = $('#add-new-event form'),
            this.$extEvents = $('#calendar-events'),
            this.$modal = $('#my-event'),
            this.$saveCategoryBtn = $('.save-category'),
            this.$calendarObj = null
    };


    /* on drop */
    CalendarApp.prototype.onDrop = function(eventObj, date) {
            var $this = this;
            // retrieve the dropped element's stored Event Object
            var originalEventObject = eventObj.data('eventObject');
            var $categoryClass = eventObj.attr('data-class');
            // we need to copy it, so that multiple events don't have a reference to the same object
            var copiedEventObject = $.extend({}, originalEventObject);
            // assign it the date that was reported
            copiedEventObject.start = date;
            if ($categoryClass)
                copiedEventObject['className'] = [$categoryClass];
            // render the event on the calendar
            $this.$calendar.fullCalendar('renderEvent', copiedEventObject, true);
            // is the "remove after drop" checkbox checked?
            if ($('#drop-remove').is(':checked')) {
                // if so, remove the element from the "Draggable Events" list
                eventObj.remove();
            }
        },
        /* on click on event */
        CalendarApp.prototype.onEventClick = function(calEvent, jsEvent, view) {
            var $this = this;
            var form = $("<form></form>");
            form.append("<label>Change event name</label>");
            form.append("<div class='input-group'><input class='form-control' type=text value='" + calEvent.title + "' /><span class='input-group-btn'><button type='submit' class='btn btn-success waves-effect waves-light'><i class='fa fa-check'></i> Save</button></span></div>");
            $this.$modal.modal({
                backdrop: 'static'
            });
            $this.$modal.find('.delete-event').show().end().find('.save-event').hide().end().find('.modal-body').empty().prepend(form).end().find('.delete-event').unbind('click').click(function() {
                $this.$calendarObj.fullCalendar('removeEvents', function(ev) {
                    return (ev._id == calEvent._id);
                });
                $this.$modal.modal('hide');
            });
            $this.$modal.find('form').on('submit', function() {
                calEvent.title = form.find("input[type=text]").val();
                $this.$calendarObj.fullCalendar('updateEvent', calEvent);
                $this.$modal.modal('hide');
                return false;
            });
        },
        /* on select */
        CalendarApp.prototype.onSelect = function(start, end, allDay) {
            var $this = this;
            $this.$modal.modal({
                backdrop: 'static'
            });
            var form = $("<form></form>");
            form.append("<div class='row'></div>");
            form.find(".row")
                .append("<div class='col-md-6'><div class='form-group'><label class='control-label'>Event Name</label><input class='form-control' placeholder='Insert Event Name' type='text' name='title'/></div></div>")
                .append("<div class='col-md-6'><div class='form-group'><label class='control-label'>Category</label><select class='form-control' name='category'></select></div></div>")
                .find("select[name='category']")
                .append("<option value='bg-danger'>Danger</option>")
                .append("<option value='bg-success'>Success</option>")
                .append("<option value='bg-primary'>Primary</option>")
                .append("<option value='bg-info'>Info</option>")
                .append("<option value='bg-warning'>Warning</option></div></div>");
            $this.$modal.find('.delete-event').hide().end().find('.save-event').show().end().find('.modal-body').empty().prepend(form).end().find('.save-event').unbind('click').click(function() {
                form.submit();
            });
            $this.$modal.find('form').on('submit', function() {
                var title = form.find("input[name='title']").val();
                var beginning = form.find("input[name='beginning']").val();
                var ending = form.find("input[name='ending']").val();
                var categoryClass = form.find("select[name='category'] option:checked").val();
                if (title !== null && title.length != 0) {
                    $this.$calendarObj.fullCalendar('renderEvent', {
                        title: title,
                        start: start,
                        end: end,
                        allDay: false,
                        className: categoryClass
                    }, true);
                    $this.$modal.modal('hide');
                } else {
                    alert('You have to give a title to your event');
                }
                return false;

            });
            $this.$calendarObj.fullCalendar('unselect');
        },
        CalendarApp.prototype.enableDrag = function() {
            //init events
            $(this.$event).each(function() {
                // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
                // it doesn't need to have a start or end
                var eventObject = {
                    title: $.trim($(this).text()) // use the element's text as the event title
                };
                // store the Event Object in the DOM element so we can get to it later
                $(this).data('eventObject', eventObject);
                // make the event draggable using jQuery UI
                $(this).draggable({
                    zIndex: 999,
                    revert: true, // will cause the event to go back to its
                    revertDuration: 0 //  original position after the drag
                });
            });
        }
    /* Initializing */
    CalendarApp.prototype.init = function(data) {
            this.enableDrag();
            /*  Initialize the calendar  */
            var date = new Date();
            var d = date.getDate();
            var m = date.getMonth();
            var y = date.getFullYear();
            var form = '';
            var today = new Date($.now());
            var bindingData = [];
            var i = 0;
        var gridData = "";

        for (var elem in data) {
                var currentStatus;
                var current = data[elem];

                if (current.status == 1) {
                    bindingData[i] = {
                        title: current.title,
                        start: current.startDate,
                        end: current.endDate,
                        className: 'bg-inactive',
                        _id: current.id
                    };
                    currentStatus = `<td><span class="badge bg-light-inactive text-inactive fw-normal">Not Signed</span></td>`;
                }
                else if (current.status == 2) {
                    bindingData[i] = {
                        title: current.title,
                        start: current.startDate,
                        end: current.endDate,
                        className: 'bg-success',
                        _id: current.id
                    };
                    currentStatus = `<td><span class="badge bg-light-success text-success fw-normal">Active</span></td>`
                }
                else if (current.status == 3) {
                    bindingData[i] = {
                        title: current.title,
                        start: current.startDate,
                        end: current.endDate,
                        className: 'bg-info',
                        _id: current.id
                    };
                    currentStatus = `<td><span class="badge bg-light-warning text-warning fw-normal">Requested Signature</span></td>`
                }
                else if (current.status == 4) {
                    bindingData[i] = {
                        title: current.title,
                        start: current.startDate,
                        end: current.endDate,
                        className: 'bg-danger',
                        _id: current.id

                    };
                    currentStatus = `<td><span class="badge bg-light-info text-info fw-normal">Confirming Payment</span></td>`;
                }
                else if (current.status == 4) {
                    bindingData[i] = {
                        title: current.title,
                        start: current.startDate,
                        end: current.endDate,
                        className: 'bg-danger',
                        _id: current.id

                    };
                    currentStatus = `<td><span class="badge bg-light-success text-success fw-normal">Completed</span></td>`;
            }
            var cProvider = current.serviceProider;
            if (cProvider == null)
                cProvider = "--";
            else
            {
                var lenght = cProvider.length;
                cProvider = cProvider.substring(0, 3) + "..." + cProvider.substring(lenght - 3, lenght);
            }
                
                gridData += `  <tr class="tableRowSelectable" onclick="GetEventData(` + current.id + `)" id="id_` + current.id +`">
                                    <td>
                                    <div class="d-flex align-items-center">
                                        <i class="ti-write gridSize"></i>
                                        <span class="ms-3 fw-normal">`+ current.title + `</span>
                                    </div>
                                    </td>
                                    <td>`+ cProvider + `</td>
                                    <td>
                                    `+ new Date(current.startDate).toLocaleDateString("en-us")   + `
                                    </td>
                                    <td>
                                        `+ new Date(current.endDate).toLocaleDateString("en-us") + `
                                    </td>
                                    `+ currentStatus + `

                                </tr>`;
            
                i++;
            }

        $("#gridBody").html(gridData);
        if (gridData === "") {
            $("#emptyGrid").show();
        }

            var $this = this;
            $this.$calendarObj = $this.$calendar.fullCalendar({
                slotDuration: '00:15:00',
                /* If we want to split day time each 15minutes */
                minTime: '08:00:00',
                maxTime: '19:00:00',
                defaultView: 'month',
                handleWindowResize: true,

                header: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'month,agendaWeek,agendaDay'
                },
                events: bindingData,
                editable: false,
                droppable: false, // this allows things to be dropped onto the calendar !!!
                eventLimit: false, // allow "more" link when too many events
                selectable: false,
                eventClick: function (calEvent, jsEvent, view) {
                    console.log(calEvent);
                    GetEventData(calEvent._id);
                }

            });

            //on new event
            this.$saveCategoryBtn.on('click', function() {
                var categoryName = $this.$categoryForm.find("input[name='category-name']").val();
                var categoryColor = $this.$categoryForm.find("select[name='category-color']").val();
                if (categoryName !== null && categoryName.length != 0) {
                    $this.$extEvents.append('<div class="calendar-events m-b-20" data-class="bg-' + categoryColor + '" style="position: relative;"><i class="fa fa-circle text-' + categoryColor + ' m-r-10" ></i>' + categoryName + '</div>')
                    $this.enableDrag();
                }

            });
        },

        //init CalendarApp
        $.CalendarApp = new CalendarApp, $.CalendarApp.Constructor = CalendarApp

}(window.jQuery),

//initializing CalendarApp
$(window).on('load', function() {

    StartCalendar();

});

function StartCalendar() {


    $.ajax({
        method: "GET",
        contentType: "application/json",
        url: "/Platform/GetUserContracts",
    }).done(async function (msg) {
        if (msg === null) {
            toastr.warning('Please connect your wallet.');
            return;
        }

        $.CalendarApp.init(msg)
    });

}