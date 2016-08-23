using Kookaburra.Domain.Model;

namespace Kookaburra.Domain.Repository
{
    public interface IVisitorRepository
    {
        Visitor AddVisitor(Visitor visitor);

        Visitor CheckForVisitor(Visitor visitor);
    }
} 