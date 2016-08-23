using Kookaburra.Domain.Model;

namespace Kookaburra.Domain.Repository
{
    public interface IVisitorRepository
    {
        Visitor AddVisitor(Visitor visitor);

        Visitor CheckForVisitor(string name, string email, string sessionId);
    }
}