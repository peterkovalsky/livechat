using System.Collections.Generic;

namespace Kookaburra.Services.Chats
{
    public class OperatorInfoResponse
    {
        public string Name { get; set; }

        public List<string> ConnectionIds { get; set; }
    }
}