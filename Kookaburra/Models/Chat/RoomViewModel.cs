using Kookaburra.Domain.Common;
using Kookaburra.Models.Account;

namespace Kookaburra.ViewModels.Chat
{
    public class RoomViewModel
    {
        public string CompanyId { get; set; }

        public string OperatorName { get; set; }

        public int OperatorId { get; set; }

        public long? ChatId { get; set; }

        public AccountStatusType AccountStatus { get; set; }

        public TrialExpiredViewModel TrialExpiredViewModel { get; set; }
    }
}