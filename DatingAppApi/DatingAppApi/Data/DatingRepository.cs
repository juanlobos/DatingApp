using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DatingAppApi.Helpers;

namespace DatingAppApi.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(z => z.LikerId == userId && z.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(u => u.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var foto = await _context.Photos.FirstOrDefaultAsync(z => z.Id == id);
            return foto;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(z => z.Id == id);
            return user;
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(z => z.Likers).Include(z => z.Likees).FirstOrDefaultAsync(z => z.Id == id);
            if (likers)
            {
                return user.Likers.Where(z => z.LikeeId == id).Select(n => n.LikerId);
            }
            else
            {
                return user.Likees.Where(g => g.LikerId == id).Select(x => x.LikeeId);
            }

        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(z => z.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);
            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(z => userLikers.Contains(z.Id));
            }
            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(z => userLikees.Contains(z.Id));
            }
            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(i => i.DateOfBirth >= minDob && i.DateOfBirth <= maxDob);
            }
            if (!String.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderBy(z => z.Created);
                        break;
                    default:
                        users = users.OrderBy(z => z.LastActive);
                        break;
                }
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(u => u.Sender).ThenInclude(u => u.Photos).
            Include(u => u.Recipient).ThenInclude(u => u.Photos).AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(z => z.RecipientId== messageParams.UserId && z.RecipientDeleted==false);
                    break;
                case "OutBox":
                    messages = messages.Where(z => z.SenderId == messageParams.UserId && z.SenderDeleted== false);
                    break;
                default:
                    messages = messages.Where(z => z.RecipientId == messageParams.UserId && z.RecipientDeleted == false && z.IsRead == false);
                    break;
            }
            messages = messages.OrderByDescending(x => x.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessagesThread(int userId, int recipientId)
        {
            var messages = await _context.Messages.Include(u => u.Sender).ThenInclude(u => u.Photos).
            Include(u => u.Recipient).ThenInclude(u => u.Photos).Where(m => m.RecipientId == userId && m.RecipientDeleted==false
             && m.SenderId == recipientId
            || m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted==false).OrderByDescending(m =>m.MessageSent).ToListAsync();
            return messages;
        }
    }
}