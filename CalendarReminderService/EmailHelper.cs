using System.Collections.Generic;
using System.Net.Mail;
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
            var emails = new List<MailAddress> { new MailAddress("ken.burkhardt@gmail.com"), new MailAddress("jakehmiller@gmail.com") };
            
            var email = Email
                       .From("assignmentreminder@gmail.com","Congregation Assignment Reminder")
                       .To(calendarEvent.EventDetail.Email)
                       //.CC(emails)
                       .Subject("Congregation Assignment Reminder: " + calendarEvent.Title  + " " + calendarEvent.Date.ToShortDateString())
                       .UsingClient(_smtpClient)
                       .UsingTemplateFromFile("emailtemplate.txt", new { Name = calendarEvent.EventDetail.Name, Assignment = calendarEvent.EventDetail.Assignment, Date = calendarEvent.Date.ToShortDateString() });

            email.Send();
        }
    }
}