using System.Threading.Tasks;

namespace Kookaburra.Domain.Command
{
    public interface ICommand
    {
        string OperatorIdentity { get; }
    }

    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task ExecuteAsync(TCommand command);
    }   
}