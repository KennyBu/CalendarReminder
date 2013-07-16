using System;
using Quartz;
using Quartz.Impl;
using Topshelf;

namespace CalendarReminderService
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>                                 
            {
                x.Service<CalendarReminderWindowsService>(s =>                       
                {
                    s.ConstructUsing(name => new CalendarReminderWindowsService());    
                    s.WhenStarted(tc => tc.Start());              
                    s.WhenStopped(tc => tc.Stop());              
                });
                x.RunAsLocalSystem();                           

                x.SetDescription("Calendar Reminder Service");        
                x.SetDisplayName("Calendar Reminder Service");                      
                x.SetServiceName("CalendarReminderService");                      
            });  
            
        }
    }
}
