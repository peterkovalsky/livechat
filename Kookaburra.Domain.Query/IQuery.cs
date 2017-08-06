namespace Kookaburra.Domain.Query
{
    public interface IQuery<TResult>
    {
        string OperatorIdentity { get; }

        string AccountKey { get; }
    }

    public interface IQueryHandler<in TQuery, out TResult> where TQuery : IQuery<TResult>
    {
        TResult ExecuteAsync(TQuery query);
    }   
}