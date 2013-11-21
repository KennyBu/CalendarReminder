using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalendarReminder;
using PetaPoco;

namespace CalendarReminderService
{
    public class AssigneeManager
    {
        private readonly Database _db;

        public AssigneeManager(Database db)
        {
            _db = db;
        }        
        
        public List<Assignee> GetKMList()
        {
            var list = _db.Fetch<Assignee>("WHERE SendKM=@0", 1);

            return list;
        }
    }
}
