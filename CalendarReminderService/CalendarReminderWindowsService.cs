using System;
using Quartz;
using Quartz.Impl;

namespace CalendarReminderService
{
    public interface IWindowsService
    {
        void Start();
        void Stop();
    }

    public class CalendarReminderWindowsService : IWindowsService
    {
        private readonly ISchedulerFactory _schedFact;
        private readonly IScheduler _sched;

        public CalendarReminderWindowsService()
        {
            _schedFact = new StdSchedulerFactory();
            _sched = _schedFact.GetScheduler();
        }
        
        public void Start()
        {
            var jobDetail = JobBuilder.Create<CalendarReminderJob>()
                .WithDescription("myJob")
                .Build();

            var trigger = TriggerBuilder.Create().ForJob(jobDetail)
                                        .WithDescription("myTrigger")
                                        //.WithCalendarIntervalSchedule(a => a.WithIntervalInMinutes(1))
                                        .WithCronSchedule("0 15 23 ? * MON")

                                        /*.WithCronSchedule(
                                            CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(22,30,DayOfWeek.Monday)
                                            .WithMisfireHandlingInstructionDoNothing()
                                            .InTimeZone(TimeZoneInfo.Local)
                                            .Build()
                                            .ToString())
                                         */
                                        .Build();
                                        

            _sched.Start();
            _sched.ScheduleJob(jobDetail, trigger);
            
        }

        public void Stop()
        {
            _sched.Shutdown();
        }
    }
}