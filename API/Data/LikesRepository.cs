using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
            
        }
        public async Task<UserLike> GetUserLike(int sourceUserID, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserID, targetUserId); // Has a composite primary key
            
        }

        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            // Getting a list of userlikes based on predicate
            // Could be user of source user, or be the target user depending on predicate
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable(); // get a list of users ordered by username. Queryable means nothing has happened with database
            var likes = _context.Likes.AsQueryable(); // won't get all likes and users because it is a queryable (not executed yet)
            
            if (predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == userId);
                users = likes.Select(like => like.TargetUser);
            }

            if (predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == userId);
                users = likes.Select(like => like.SourceUser);
            }

            return await users.Select(user => new LikeDto
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoURL = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();

        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users // See who the users has liked
                        .Include(x => x.LikedUsers)
                        .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}