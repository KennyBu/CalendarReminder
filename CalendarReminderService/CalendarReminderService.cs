using System;
using System.Collections.Generic;
using System.IO;
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
            var db = new PetaPoco.Database("AssignifyItDatabase");

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
                if (sendEmails)
                {
                    emailHelper.SendNoEventsEmail(mondayOfThisWeek.ToShortDateString());    
                }
            }

            foreach (var reminder in reminders.OrderBy(a => a.Date))
            {
                if (sendEmails)
                {
                    emailHelper.SendAssignmentReminderEmail(reminder);
                }

                _logger.Info("Event Name:" + reminder.Title);
                _logger.Info(reminder.Date.ToShortDateString());
                _logger.Info(reminder.EventDetail.Assignment);
                _logger.Info(reminder.EventDetail.Email);
                _logger.Info(reminder.EventDetail.Name);
                _logger.Info("----------------------");

            }

            try
            {

                //Email the KM's To Approved Publishers
                var manager = new AssigneeManager(db);
                var list = manager.GetKMList();

                //todo get the KM file(s)
                var location = string.Format("{0}\\KM", AppDomain.CurrentDomain.BaseDirectory);
                var finishedLocation = string.Format("{0}\\KMSent", AppDomain.CurrentDomain.BaseDirectory);

                var isExists = Directory.Exists(location);
                if (!isExists)
                    Directory.CreateDirectory(location);

                isExists = Directory.Exists(finishedLocation);
                if (!isExists)
                    Directory.CreateDirectory(finishedLocation);

                var kmFileNames = Directory.GetFiles(location);
                Directory.GetFiles(location);

                if (kmFileNames.Length == 0)
                {
                    if (sendEmails)
                    {
                        emailHelper.SendNoKMFoundEmail();
                    }
                }

                //Loop and email
                foreach (var kmFileName in kmFileNames)
                {
                    var name = Path.GetFileName(kmFileName);
                    var finishedFile = string.Format("{0}\\{1}", finishedLocation, name);

                    foreach (var assignee in list)
                    {
                        if (sendEmails)
                        {
                            emailHelper = GetNewEmailHelper(userName, password);
                            emailHelper.SendKMEmail(assignee, kmFileName);
                        }

                        _logger.Info("Event Name: Sent KM");
                        _logger.Info(assignee.Name);
                        _logger.Info(assignee.Email);
                        _logger.Info(kmFileName);
                        _logger.Info("----------------------");
                    }

                    //remove file
                    File.Move(kmFileName, finishedFile);
                }
            }
            catch (Exception exception)
            {
                _logger.Error("An Error Occurred Processing the KMs:", exception);
                throw;
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

        private EmailHelper GetNewEmailHelper(string userName, string password)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = true
            };
            var emailHelper = new EmailHelper(client);

            return emailHelper;
        }
    }
}