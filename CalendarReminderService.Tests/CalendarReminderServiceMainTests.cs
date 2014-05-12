using System.Net;
using System.Net.Mail;
using Formo;
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


        [Test]
        public void ExecuteMailTest()
        {
            dynamic config = new Configuration();
            var userName = config.UserName;
            var password = config.Password;
            const string file = "c:\\temp\\KMs\\send\\km_e-Us_E_201406.pdf";
            
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = true
            };
            var emailHelper = new EmailHelper(client);

            emailHelper.SendTestEmail("","", file);
        }


    }
}