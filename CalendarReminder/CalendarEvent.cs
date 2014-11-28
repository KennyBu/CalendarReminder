using System;

namespace CalendarReminder
{
    public interface ICalendarEvent
    {
        Guid Id { get; set; }
        string Title { get; set; }
        DateTime AssignmnetDate { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Assignment { get; set; }
    }

    public class CalendarEvent : ICalendarEvent
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime AssignmnetDate { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Assignment { get; set; }
    }
}