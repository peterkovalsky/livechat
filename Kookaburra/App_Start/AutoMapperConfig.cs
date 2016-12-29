using AutoMapper;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Models.Widget;

namespace Kookaburra.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg => {              
                cfg.CreateMap<ConversationItemViewModel, ConversationItem>();
                cfg.CreateMap<ConversationViewModel, ContinueConversationQueryResult>();
            });
        }
    }
}