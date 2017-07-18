﻿using Kookaburra.Domain.Command.OperatorMessaged;
using Kookaburra.Domain.Integration;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Command.StartVisitorChat
{
    public class StartVisitorChatCommandHandler : ICommandHandler<StartVisitorChatCommand>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;
        private readonly IGeoLocator _geoLocator;
        private readonly ICommandHandler<OperatorMessagedCommand> _operatorMessagedHandler;

        public StartVisitorChatCommandHandler(KookaburraContext context, ChatSession chatSession, IGeoLocator geoLocator, ICommandHandler<OperatorMessagedCommand> operatorMessagedHandler)
        {
            _context = context;
            _chatSession = chatSession;
            _geoLocator = geoLocator;
            _operatorMessagedHandler = operatorMessagedHandler;
        }


        public async Task ExecuteAsync(StartVisitorChatCommand command)
        {
            // record new/returning visitor
            var returningVisitor = await CheckForVisitorAsync(command.VisitorName, command.VisitorEmail, command.SessionId);

            // new visitor
            if (returningVisitor == null)
            {                         
                returningVisitor = new Visitor
                {
                    Name = command.VisitorName,
                    Email = command.VisitorEmail,             
                    SessionId = command.SessionId,
                    IpAddress = command.VisitorIP,
                };

                try
                {
                    var location = await _geoLocator.GetLocationAsync(command.VisitorIP);

                    if (location != null)
                    {
                        returningVisitor.Country = location.Country;
                        returningVisitor.CountryCode = location.CountryCode;
                        returningVisitor.Region = location.Region;
                        returningVisitor.City = location.City;
                        returningVisitor.Latitude = location.Latitude;
                        returningVisitor.Longitude = location.Longitude;
                    }
                }
                catch (Exception ex)
                {
                    // log geo location exception
                }            

                _context.Visitors.Add(returningVisitor);
            }

            var operatorSession = _chatSession.GetOperatorById(command.OperatorId);

            var conversation = new Conversation
            {
                OperatorId = operatorSession.Id,
                Visitor = returningVisitor,
                TimeStarted = DateTime.UtcNow,
                Page = command.Page
            };
            _context.Conversations.Add(conversation);      

            await _context.SaveChangesAsync();

           
            // add visitor to session
            _chatSession.AddVisitor(conversation.Id, command.OperatorId, null, returningVisitor.Id, returningVisitor.Name, returningVisitor.SessionId);

            // add greeting if needed           
            await _operatorMessagedHandler.ExecuteAsync(new OperatorMessagedCommand
                (
                    command.SessionId,
                    DefaultSettings.CHAT_GREETING,
                    DateTime.UtcNow,
                    command.OperatorIdentity
                ));
        }

        private async Task<Visitor> CheckForVisitorAsync(string name, string email, string sessionId)
        {
            var existingVisitor = await _context.Visitors
                .Where(v => v.SessionId == sessionId)
                .SingleOrDefaultAsync();

            return existingVisitor;
        }
    }
}