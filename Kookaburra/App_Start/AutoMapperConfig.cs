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
                cfg.CreateMap<ConversationItem, MessageViewModel>();
                cfg.CreateMap<ContinueConversationQueryResult, ConversationViewModel>();
            });
        }
    }
}