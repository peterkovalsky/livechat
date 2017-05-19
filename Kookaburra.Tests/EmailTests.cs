using Kookaburra.Email;
using Kookaburra.Email.Public.SignUpWelcome;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kookaburra.Tests
{
    [TestClass]
    public class EmailTests
    {
        private readonly IMailer _mailer;
        private readonly AddressInfo _from;
        private readonly AddressInfo _to;        

        public EmailTests()
        {
            var sender = new DefaultEmailSender("mail.kookaburra.chat", "info@kookaburra.chat", "Private123!");
            _mailer = new Mailer(sender);
            _from = new AddressInfo("Kookaburra Chat", "info@kookaburra.chat");
            _to = new AddressInfo("peter.kovalskyy@gmail.com");
        } 

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestSignUpWelcomeEmail()
        {            
            var model = new SignUpWelcomeEmail
            {
                FirstName = "John"
            };

            _mailer.SendEmail(_from, _to, model);
        }
    }
}