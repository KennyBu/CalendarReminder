using CalendarReminder;
using Ninject.Modules;

namespace CalendarReminderService
{
    public class CalendarReminderModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ICalenderHelper>().To<CalenderHelper>();
        }
    }
}