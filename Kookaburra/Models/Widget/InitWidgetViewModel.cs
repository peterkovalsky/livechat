using Newtonsoft.Json;

namespace Kookaburra.Models.Widget
{
    public class InitWidgetViewModel
    {
        [JsonProperty("step")]
        public string Step { get; set; }

        [JsonProperty("visitorName")]
        public string VisitorName { get; set; }

        [JsonProperty("visitorEmail")]
        public string VisitorEmail { get; set; }

        [JsonProperty("resumedChat")]
        public ConversationViewModel ResumedChat { get; set; }       
    }
}