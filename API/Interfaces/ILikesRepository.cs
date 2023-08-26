using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserID, int targetUserId);
        Task<AppUser> GetUserWithLikes(int userId); // will include related entities
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}