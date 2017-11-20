using System.Collections.Generic;

namespace Kookaburra.Services.Chats
{
    public class CurrentSessionResponse
    {
        public string VisitorName { get; set; }

        public List<string> VisitorConnectionIds { get; set; }

        public string VisitorIdentity { get; set; }

        public string OperatorName { get; set; }

        public List<string> OperatorConnectionIds { get; set; }
    }
}