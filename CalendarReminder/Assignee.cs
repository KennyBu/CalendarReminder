using System;

namespace CalendarReminder
{
    public class Assignee
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool SendKM { get; set; }
    }
}
