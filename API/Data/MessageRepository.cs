using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;


namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)

        {
            var query = _context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable(); // Order the messages by the date sent

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false), // If getting inbox, get messages where recipient username is the username
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false), // If getting outbox (sent), get messages where sender is the username
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.DateRead == null && u.RecipientDeleted == false) // if no box specified, get the messages that are unread and the recipient is user

            };
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize); // Call PagedList.CreateAsync with dto as the type
            
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = await _context.Messages
                .Include(m => m.Sender).ThenInclude(u => u.Photos)
                .Include(m => m.Recipient).ThenInclude(u => u.Photos)
                .Where(
                    m => // Get the thread where they are sending or receiving to/from each other only
                    (m.RecipientUsername == currentUserName && m.SenderUsername == recipientUserName && m.RecipientDeleted == false) || // Get the non-deleted messages to the current user (inbox)
                    (m.RecipientUsername == recipientUserName && m.SenderUsername == currentUserName && m.SenderDeleted == false) // Get the non-deleted messages from the current user (outbox)
                )
                .OrderBy(m => m.MessageSent)
                .ToListAsync();
            var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUserName).ToList(); // filter against in-memory data

            if (unreadMessages.Any())
            {
                foreach(Message message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow; // mark message as read when received by the recipient
                }
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages); // map to message DTO

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}