using System;
using System.Net.Mail;
using System.Net.Mime;
using CalendarReminder;
using FluentEmail;

namespace CalendarReminderService
{
    public class EmailHelper
    {
        private readonly SmtpClient _smtpClient;

        public EmailHelper(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public void SendAssignmentReminderEmail(CalendarEvent calendarEvent)
        {
            var email = Email
                       .From("assignmentreminder@gmail.com","Congregation Assignment Reminder")
                       .To(calendarEvent.EventDetail.Email)
                       .Subject("Congregation Assignment Reminder: " + calendarEvent.Title  + " " + calendarEvent.Date.ToShortDateString())
                       .UsingClient(_smtpClient)
                       .UsingTemplateFromFile("emailtemplate.txt", new { Name = calendarEvent.EventDetail.Name, Assignment = calendarEvent.EventDetail.Assignment, Date = calendarEvent.Date.ToShortDateString() });

            if (calendarEvent.EventDetail.Assignment.Contains("Tidying Assignment") ||
                calendarEvent.EventDetail.Assignment.Contains("Cleaning Assignment"))
            {
                var location = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\Lists\\GroupCleaningReminders.pdf");
                var attachment = new Attachment(location, MediaTypeNames.Application.Pdf);
                email.Attach(attachment);
            }

            email.Send();
        }

        public void SendNoEventsEmail(string mondayOfThisWeek)
        {
            var email = Email
                       .From("assignmentreminder@gmail.com", "Congregation Assignment Reminder")
                       .To("ken.burkhardt@gmail.com")
                       .Subject("Congregation Assignment Reminder")
                       .UsingClient(_smtpClient)
                       .UsingTemplateFromFile("noassignmentemailtemplate.txt", new { Name = "Ken", Date = mondayOfThisWeek});

            email.Send();
        }
    }
}