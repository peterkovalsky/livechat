using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kookaburra.Domain.Query.Handler;
using Kookaburra.Repository;
using Kookaburra.Domain;

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

            session.AddOperator(1, "John", Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            //session.AddVisitor("",);

            var handler = new ContinueConversationQueryHandler(context, session);
        }
    }
}
