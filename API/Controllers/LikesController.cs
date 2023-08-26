using API.Data;
using API.Entities;
using API.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly LikesRepository _likesRepository;
        private readonly UserRepository _userRepository;
        public LikesController(UserRepository userRepository, LikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;

        }

        [HttpPost("{username}")] // For when a user is about to like another user
        // We're just creating a link on the join table
        // Not actually returning anything from this method - we don't need to
        public async Task<ActionResult> AddLike(string username)
        {
            int sourceUserID = int.Parse(User.GetUserId());
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserID);

            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserID, likedUser.Id);
            if (userLike != null) return BadRequest("You already like this user");

            userLike = new Entities.UserLike
            {
                SourceUserId = sourceUserID,
                TargetUserId = likedUser.Id
            }; // Create a new userLike (just needs composite key)
            

            sourceUser.LikedUsers.Add(userLike);
            if (!await _userRepository.SaveAllAsync()) return BadRequest("Failed to like user"); // If we have trouble saving
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
        {
            var users = await _likesRepository.GetUserLikes(predicate, int.Parse(User.GetUserId()));

            return Ok(users);
        }
    }
}