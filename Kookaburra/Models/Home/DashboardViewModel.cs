using Kookaburra.Domain.Common;
using Kookaburra.Models.Account;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.Home
{
    public class DashboardViewModel
    {
        [JsonProperty("totalCurrentChats")]
        public int TotalCurrentChats { get; set; }

        [JsonProperty("newOfflineMessages")]
        public int NewOfflineMessages { get; set; }      

        [JsonProperty("currentChats")]
        public List<LiveChatViewModel> CurrentChats { get; set; } = new List<LiveChatViewModel>();

        public AccountStatusType AccountStatus { get; set; }

        public TrialExpiredViewModel TrialExpiredViewModel { get; set; }

        public List<DailyChatsViewModel> DailyChats { get; set; }

        public int TrialDaysLeft { get; set; }

        public int TotalChats { get; set; }
    }

    public class LiveChatViewModel
    {
        [JsonProperty("chatId")]
        public long ChatId { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("visitorName")]
        public string VisitorName { get; set; }

        [JsonProperty("timeStarted")]
        public double TimeStarted { get; set; }

        [JsonProperty("page")]
        public string Page { get; set; }
    }
}