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
                       .To(calendarEvent.Email)
                       .Subject("Congregation Assignment Reminder: " + calendarEvent.Title + " " + calendarEvent.AssignmnetDate.ToShortDateString())
                       .UsingClient(_smtpClient)
                       .UsingTemplateFromFile("emailtemplate.txt", new { Name = calendarEvent.Name, Assignment = calendarEvent.Assignment, Date = calendarEvent.AssignmnetDate.ToShortDateString() });

            if (calendarEvent.Assignment.Contains("Tidying Assignment") ||
                calendarEvent.Assignment.Contains("Cleaning Assignment"))
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

        public void SendKMEmail(Assignee assignee, string kmFullFileName)
        {
            var email = Email
                       .From("assignmentreminder@gmail.com", "Congregation Assignment Reminder")
                       .To(assignee.Email)
                       .Subject("New Kingdom Ministry")
                       .UsingClient(_smtpClient)
                       .UsingTemplateFromFile("km.txt", new {assignee.Name });
            
            var attachment = new Attachment(kmFullFileName);
            email.Attach(attachment);

            email.Send();

            email.Dispose();
        }

        public void SendNoKMFoundEmail()
        {
            var email = Email
                       .From("assignmentreminder@gmail.com", "Congregation Assignment Reminder")
                       .To("ken.burkhardt@gmail.com")
                       .Subject("New Kingdom Ministry")
                       .UsingClient(_smtpClient)
                       .UsingTemplateFromFile("nokmfoundemailtemplate.txt", new { Name = "Ken"});

            email.Send();
        }

        public void SendTestEmail(string emailAddress, string name, string file)
        {
            var email = Email
                       .From("assignmentreminder@gmail.com", "Congregation Assignment Reminder")
                       .To(emailAddress)
                       .Subject("New Kingdom Ministry Test")
                       .UsingClient(_smtpClient)
                       .UsingTemplateFromFile("km.txt", new { Name = name });

            var attachment = new Attachment(file);
            email.Attach(attachment);

            email.Send();

            email.Dispose();
        }
    }
}