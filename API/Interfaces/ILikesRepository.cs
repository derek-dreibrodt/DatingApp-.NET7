using API.Entities;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserID, int targetUserId);
        Task<AppUser> GetUserWithLikes(int userId); // will include related entities
        Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId);
    }
}