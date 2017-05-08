using Kookaburra.Domain;
using Kookaburra.Domain.ResumeVisitorChat;
using Kookaburra.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Kookaburra.Tests
{
    [TestClass]
    public class QueriesTests
    {
        [TestMethod]
        public void TestContinueConversationQuery()
        {
            var context = new KookaburraContext("Data Source=.;Initial Catalog=kookaburra;uid=sa;password=bazz1983petro;MultipleActiveResultSets=True");
            var session = new ChatSession();

            session.AddOrUpdateOperator(1, Guid.NewGuid().ToString(), "John", Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            //session.AddVisitor("",);

            var handler = new ResumeVisitorChatQueryHandler(context, session);
        }
    }
}
