using System;
using System.Collections.Generic;
using PetaPoco;

namespace CalendarReminder
{
    public interface ICalendarReminder
    {
        List<CalendarEvent> GetEvents(DateTime from, DateTime to);
    }

    public class SqlCalendarReminder : ICalendarReminder
    {
        private readonly Database _db;

        public SqlCalendarReminder(Database db)
        {
            _db = db;
        }

        public List<CalendarEvent> GetEvents(DateTime from, DateTime to)
        {
            var list = _db.Fetch<CalendarEvent>("WHERE AssignmnetDate >=@0 AND AssignmnetDate <=@1", from, to);

            return list;
        }
    }
}