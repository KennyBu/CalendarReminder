using System;

namespace CalendarReminder
{
    public interface ICalendarEvent
    {
        string Title { get; set; }
        DateTime Date { get; set; }
        IEventDetail EventDetail { get; set; }
    }

    public class CalendarEvent : ICalendarEvent
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public IEventDetail EventDetail { get; set; }
    }
}