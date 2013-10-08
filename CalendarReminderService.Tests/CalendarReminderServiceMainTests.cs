using NUnit.Framework;

namespace CalendarReminderService.Tests
{
    [TestFixture]
    public class CalendarReminderServiceMainTests
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            
        }

        [TestFixtureTearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Execute()
        {
            var service = new CalendarReminderService();
            service.Execute();

        }
    }
}