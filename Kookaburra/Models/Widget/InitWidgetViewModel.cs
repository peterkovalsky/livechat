using Kookaburra.Common;
using Newtonsoft.Json;

namespace Kookaburra.Models.Widget
{
    public class InitWidgetViewModel
    {
        public InitWidgetViewModel(WidgetStepType step)
        {
            Step = step.ToString();
        }

        [JsonProperty("step")]
        public string Step { get; set; }

        [JsonProperty("visitorName")]
        public string VisitorName { get; set; }

        [JsonProperty("visitorEmail")]
        public string VisitorEmail { get; set; }

        [JsonProperty("resumedChat")]
        public ConversationViewModel ResumedChat { get; set; }

        [JsonProperty("cookieName")]
        public string CookieName { get; set; }

        [JsonProperty("newVisitorId")]
        public string NewVisitorKey { get; set; }
    }
}