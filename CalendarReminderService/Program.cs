using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using CalendarReminder;
using FluentEmail;
using Ninject;
using Ninject.Modules;
using ServiceStack.Text;

namespace CalendarReminderService
{
    class Program
    {
        static void Main(string[] args)
        {

            var today = DateTime.Now;
            //var today = DateTime.Parse("06/25/2012");

            var mondayOfThisWeek = GetThisWeeksMondayDate(today);
            var testMondaysDate = mondayOfThisWeek;
            var startRange = testMondaysDate;
            var endRange = testMondaysDate.AddDays(6);

            var userName = ConfigurationManager.AppSettings["UserName"];
            var password = ConfigurationManager.AppSettings["Password"];
            var applicationName = ConfigurationManager.AppSettings["ApplicationName"];
            var calendarName = ConfigurationManager.AppSettings["CalendarName"];

            var reminders = new List<CalendarEvent>();

            NinjectModule module = new CalendarReminderModule();
            var kernel = new StandardKernel(module);

            var helper = kernel.Get<ICalenderHelper>();
            
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

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = true
            };
            var emailHelper = new EmailHelper(client);


            foreach (var reminder in reminders.OrderBy(a => a.Date))
            {
               //emailHelper.SendAssignmentReminderEmail(reminder);
                
                Console.WriteLine("Event Name:" + reminder.Title);
                Console.WriteLine();
                Console.WriteLine(reminder.Date.ToShortDateString());
                Console.WriteLine(reminder.EventDetail.Assignment);
                Console.WriteLine(reminder.EventDetail.Email);
                Console.WriteLine(reminder.EventDetail.Name);
                Console.WriteLine("----------------------");
                
            }

            Console.WriteLine("Press enter to continue...");
            
            Console.Read();

        }

        private static DateTime GetThisWeeksMondayDate(DateTime currentDate)
        {
            var currentDay = currentDate.DayOfWeek.ToString();

            if (currentDay.Equals("Monday"))
                return currentDate;

            var dayBefore = currentDate.AddDays(-1);

            return GetThisWeeksMondayDate(dayBefore);
        }
    }

    
}
