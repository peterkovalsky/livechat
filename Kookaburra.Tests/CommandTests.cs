using Kookaburra.Domain;
using Kookaburra.Domain.Command.TimeoutConversations;
using Kookaburra.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Kookaburra.Tests
{
    [TestClass]
    public class CommandTests
    {
        [TestMethod]
        public async Task TestTimeoutConversationsCommand()
        {
            var context = new KookaburraContext("Data Source=.;Initial Catalog=kookaburra;uid=sa;password=bazz1983petro;MultipleActiveResultSets=True");
            var session = new ChatSession();

            var timeoutHandler = new TimeoutConversationsCommandHandler(context, session);

            await timeoutHandler.ExecuteAsync(new TimeoutConversationsCommand(30));
        }
    }
}