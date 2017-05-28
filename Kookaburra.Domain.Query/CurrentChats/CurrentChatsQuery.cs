namespace Kookaburra.Domain.Query.CurrentChats
{
    public class CurrentChatsQuery : IQuery<CurrentChatsQueryResult>
    {
        public CurrentChatsQuery(string operatorIdentity)
        {
            OperatorIdentity = operatorIdentity;
        }

        public string OperatorIdentity { get; private set; }
    }
}