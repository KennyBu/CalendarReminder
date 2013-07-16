using Quartz;

namespace CalendarReminderService
{
    public class CalendarReminderJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var service = new CalendarReminderService();
            service.Execute();
        }
    }
}