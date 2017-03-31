namespace Kookaburra.Domain.Command
{
    public interface ICommand
    {
        string OperatorIdentity { get; }
    }

    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        void Execute(TCommand command);
    }

    public interface ICommandDispatcher
    {
        void Execute<TCommand>(TCommand command)
            where TCommand : ICommand;
    }
}