using AutoMapper;
using Kookaburra.Common;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Models;
using Kookaburra.Models.Chat;
using Kookaburra.Models.History;
using Kookaburra.Models.Offline;
using Kookaburra.Models.Widget;

namespace Kookaburra.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg => {              
                cfg.CreateMap<MessageResult, MessageViewModel>()
                    .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time.JsDateTime()));

                cfg.CreateMap<ContinueConversationQueryResult, ConversationViewModel>()
                    .ForMember(dest => dest.VisitorName, opt => opt.MapFrom(src => src.VisitorInfo.Name))
                    .ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => src.OperatorInfo.Name));

                cfg.CreateMap<CurrentChatsQueryResult, OperatorCurrentChatsViewModel>();
                cfg.CreateMap<ChatInfoResult, ChatInfo>();      

                cfg.CreateMap<ConversationResult, OperatorConversationViewModel>()
                    .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.VisitorInfo.SessionId))
                    .ForMember(dest => dest.VisitorName, opt => opt.MapFrom(src => src.VisitorInfo.Name))
                    .ForMember(dest => dest.Location, opt => opt.MapFrom(src => $"{src.VisitorInfo.Country}, {src.VisitorInfo.City}"))
                    .ForMember(dest => dest.CurrentUrl, opt => opt.MapFrom(src => src.VisitorInfo.CurrentUrl))
                    .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.VisitorInfo.StartTime.JsDateTime()))
                    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.VisitorInfo.Latitude))
                    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.VisitorInfo.Longitude));

                cfg.CreateMap<OfflineMessagesQueryResult, OfflineMessagesViewModel>();

                cfg.CreateMap<OfflineMessageResult, LeftMessageViewModel>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.VisitorName))
                    .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.City))
                    .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.TimeSent.JsDateTime()));

                cfg.CreateMap<ResumeOperatorQueryResult, CurrentConversationsViewModel>();

                cfg.CreateMap<ChatHistoryQueryResult, ChatHistoryViewModel>();
                cfg.CreateMap<ConversationItemQueryResult, ConversationItemViewModel>();

            });
        }
    }
}