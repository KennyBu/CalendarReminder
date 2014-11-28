using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using CalendarReminder;
using Formo;
using NLog;

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
            
            var sendEmails = config.SendEmails<bool?>(false);
            var db = new PetaPoco.Database("AssignifyItDatabase");

            var calendarReminder = new SqlCalendarReminder(db);

            var reminders = calendarReminder.GetEvents(startRange, endRange);

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
                if (sendEmails)
                {
                    emailHelper.SendNoEventsEmail(mondayOfThisWeek.ToShortDateString());    
                }
            }

            foreach (var reminder in reminders.OrderBy(a => a.AssignmnetDate))
            {
                if (sendEmails)
                {
                    emailHelper.SendAssignmentReminderEmail(reminder);
                }

                _logger.Info("Event Name:" + reminder.Title);
                _logger.Info(reminder.AssignmnetDate.ToShortDateString());
                _logger.Info(reminder.Assignment);
                _logger.Info(reminder.Email);
                _logger.Info(reminder.Name);
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