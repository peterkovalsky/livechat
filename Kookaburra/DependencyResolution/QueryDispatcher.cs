using Kookaburra.Domain.Query;
using System;
using System.Web.Mvc;

namespace Kookaburra.DependencyResolution
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IDependencyResolver _resolver;

        public QueryDispatcher()
        {
            _resolver = DependencyResolver.Current;
        }

        public TResult Execute<TQuery, TResult>(TQuery query)
            where TQuery : IQuery<TResult>
        {
            if (query == null)
            {
                throw new ArgumentNullException("Query doesn't have a reference to an instance of an object");
            }

            var handler = _resolver.GetService<IQueryHandler<TQuery, TResult>>();

            if (handler == null)
            {
                throw new QueryHandlerNotFoundException(typeof(TQuery));
            }

            return handler.Execute(query);
        }
    }
}