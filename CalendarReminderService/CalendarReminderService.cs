using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using CalendarReminder;
using Formo;
using NLog;
using Ninject;
using Ninject.Modules;
using ServiceStack.Text;

namespace CalendarReminderService
{
    public class CalendarReminderService
    {
        private static Logger _logger;

        public CalendarReminderService()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Execute()
        {
            var today = DateTime.Now;
            var mondayOfThisWeek = GetThisWeeksMondayDate(today);
            var testMondaysDate = mondayOfThisWeek;
            var startRange = testMondaysDate;
            var endRange = testMondaysDate.AddDays(6);

            dynamic config = new Configuration();
            var userName = config.UserName;
            var password = config.Password;
            var applicationName = config.ApplicationName;
            var calendarName = config.CalendarName;
            var sendEmails = config.SendEmails<bool?>(false);

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

            _logger.Info("{0} assigment(s) to be sent for the week of {1}", (object) reminders.Count,
                (object) mondayOfThisWeek.ToShortDateString());

            if (reminders.Count == 0)
            {
                emailHelper.SendNoEventsEmail(mondayOfThisWeek.ToShortDateString());
            }

            foreach (var reminder in reminders.OrderBy(a => a.Date))
            {
                if (sendEmails)
                    emailHelper.SendAssignmentReminderEmail(reminder);

                _logger.Info("Event Name:" + reminder.Title);
                _logger.Info(reminder.Date.ToShortDateString());
                _logger.Info(reminder.EventDetail.Assignment);
                _logger.Info(reminder.EventDetail.Email);
                _logger.Info(reminder.EventDetail.Name);
                _logger.Info("----------------------");

            }
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