using System.Collections.Generic;

namespace Kookaburra.Services.Chats
{
    public class AvailableOperatorResponse
    {
        public int OperatorId { get; set; }

        public string OperatorName { get; set; }

        public List<string> OperatorConnectionIds { get; set; }
    }
}