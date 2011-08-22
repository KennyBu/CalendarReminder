using System;
using System.Collections.Generic;
using System.Linq;
using Google.GData.Calendar;

namespace CalendarReminder
{
    public interface ICalenderHelper
    {
        string ApplicationName { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        CalendarService GetService(string applicationName, string userName, string password);
        IEnumerable<EventEntry> GetAllEvents(CalendarService service, DateTime? startData);

    }

    public class CalenderHelper : ICalenderHelper
    {
        public string ApplicationName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        
        public CalendarService GetService(string applicationName, string userName, string password)
        {
            var service = new CalendarService(applicationName);
            service.setUserCredentials(userName, password);
            return service;
        }

        public IEnumerable<EventEntry> GetAllEvents(CalendarService service, DateTime? startData)
        {
            // Create the query object:
            var query = new EventQuery
                            {
                                Uri = new Uri("http://www.google.com/calendar/feeds/" +
                                              service.Credentials.Username + "/private/full")
                            };
            if (startData != null)
                query.StartTime = startData.Value;

            // Tell the service to query:
            var calFeed = service.Query(query);
            return calFeed.Entries.Cast<EventEntry>();
        }

        public IEnumerable<EventEntry> GetEventsRange(CalendarService service, DateTime startDate, DateTime endDate, string calendarName)
        {
            // Create the query object:
            var query = new EventQuery
                            {
                                Uri = new Uri("http://www.google.com/calendar/feeds/" + calendarName + "/private/full"),
                                StartTime = startDate,
                                EndTime = endDate
                            };

            // Tell the service to query:
            var calFeed = service.Query(query);
            return calFeed.Entries.Cast<EventEntry>();
        }
    }
}