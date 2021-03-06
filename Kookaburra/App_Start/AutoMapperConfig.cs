﻿using AutoMapper;
using Kookaburra.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Models;
using Kookaburra.Models.Chat;
using Kookaburra.Models.History;
using Kookaburra.Models.Home;
using Kookaburra.Models.Offline;
using Kookaburra.Models.Widget;
using Kookaburra.Services.Chats;

namespace Kookaburra.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize(cfg => {              
                cfg.CreateMap<MessageResponse, MessageViewModel>();

                cfg.CreateMap<ResumeVisitorChatResponse, ConversationViewModel>()
                    .ForMember(dest => dest.VisitorName, opt => opt.MapFrom(src => src.VisitorInfo.Name))
                    .ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => src.OperatorInfo.Name));

                cfg.CreateMap<CurrentChatsResponse, OperatorCurrentChatsViewModel>();
                cfg.CreateMap<ChatInfoResponse, ChatInfo>();      

                cfg.CreateMap<ConversationResponse, OperatorConversationViewModel>()
                    .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.VisitorInfo.SessionId))
                    .ForMember(dest => dest.VisitorName, opt => opt.MapFrom(src => src.VisitorInfo.Name))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.VisitorInfo.Email))
                    .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.VisitorInfo.Country))
                    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.VisitorInfo.City))
                    .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.VisitorInfo.Region))
                    .ForMember(dest => dest.CurrentUrl, opt => opt.MapFrom(src => src.VisitorInfo.CurrentUrl))
                    .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.VisitorInfo.StartTime.JsDateTime()))
                    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.VisitorInfo.Latitude))
                    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.VisitorInfo.Longitude));              

                cfg.CreateMap<OfflineMessage, LeftMessageViewModel>()              
                    .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.DateSent.JsDateTime()))
                    .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Page))
                    .ForMember(dest => dest.VisitorName, opt => opt.MapFrom(src => src.Visitor.Name))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Visitor.Email))
                    .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Visitor.Country))
                    .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.Visitor.CountryCode))
                    .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.Visitor.Region))
                    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Visitor.City));

                cfg.CreateMap<ChatHistoryResponse, ChatHistoryViewModel>();
                cfg.CreateMap<ConversationItemResponse, ConversationItemViewModel>();

                cfg.CreateMap<TranscriptResponse, TranscriptViewModel>();
                cfg.CreateMap<VisitorResponse, VisitorViewModel>();

                cfg.CreateMap<Conversation, LiveChatViewModel>()
                    .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.Visitor.CountryCode))
                    .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Visitor.Country))
                    .ForMember(dest => dest.VisitorName, opt => opt.MapFrom(src => src.Visitor.Name))
                    .ForMember(dest => dest.TimeStarted, opt => opt.MapFrom(src => src.TimeStarted.JsDateTime()))
                    .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.Page));

                cfg.CreateMap<OfflineViewModel, OfflineMessage>();
                cfg.CreateMap<OfflineViewModel, Visitor>();

                cfg.CreateMap<ChatsPerDayResponse, DailyChatsViewModel>();                
            });
        }
    }
}