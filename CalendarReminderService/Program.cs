using System;
using System.Configuration;
using System.Collections.Generic;
using CalendarReminder;
using ServiceStack.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            var testMondaysDate = DateTime.Parse("08/15/2011");
            var startRange = testMondaysDate;
            var endRange = testMondaysDate.AddDays(6);

            var userName = ConfigurationManager.AppSettings["UserName"];
            var password = ConfigurationManager.AppSettings["Password"];
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var calendarName = ConfigurationManager.AppSettings["CalendarName"];

            var reminders = new List<CalendarEvent>();
            

            var helper = new CalenderHelper();
            var service = helper.GetService(applicationName, userName, password);
            var events = helper.GetEventsRange(service, startRange, endRange,
                calendarName);
            foreach (var eventEntry in events)
            {
                var calendarEvent = new CalendarEvent
                                        {
                                            Title = eventEntry.Title.Text,
                                            Date = eventEntry.Times[0].StartTime,
                                            EventDetail =
                                                JsonSerializer.DeserializeFromString<EventDetail>(
                                                    eventEntry.Content.Content)
                                        };

                reminders.Add(calendarEvent);
            }

            foreach (var reminder in reminders)
            {
                Console.WriteLine("Event Name:" + reminder.Title);
                Console.WriteLine();
                Console.WriteLine(reminder.Date.ToShortDateString());
                Console.WriteLine(reminder.EventDetail.Assignment);
                Console.WriteLine(reminder.EventDetail.Email);
                Console.WriteLine(reminder.EventDetail.Name);
                Console.WriteLine("----------------------");
            }

            Console.Read();

        }
    }
}
