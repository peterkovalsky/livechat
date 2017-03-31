namespace Kookaburra.Domain.Query
{
    public interface IQuery<TResult>
    {
        string OperatorIdentity { get; }
    }

    public interface IQueryHandler<in TQuery, out TResult>
        where TQuery : IQuery<TResult>
    {
        TResult Execute(TQuery query);
    }

    public interface IQueryDispatcher
    {
        TResult Execute<TQuery, TResult>(TQuery query)
            where TQuery : IQuery<TResult>;
    }
}