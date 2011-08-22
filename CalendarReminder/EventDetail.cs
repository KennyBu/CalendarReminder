namespace CalendarReminder
{
    public interface IEventDetail
    {
        string Name { get; set; }
        string Email { get; set; }
        string Assignment { get; set; }
    }
    
    public class EventDetail : IEventDetail
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Assignment { get; set; }
    }
}