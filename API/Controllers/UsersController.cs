using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet] 
        //[Authorize((Roles="Admin"))]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams) // We need to convert query string to UserParams
        {
            var currentUser = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            userParams.CurrentUsername = currentUser.UserName;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = currentUser.Gender == "male" ? "female" : "male";
            }

            

            PagedList<MemberDto> users = await _userRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(new PaginationHeader(
                users.CurrentPage, 
                users.PageSize, 
                users.TotalCount, 
                users.TotalPages));

            return Ok(users); // Returns 200 OK
        }

        
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            
            MemberDto member = await _userRepository.GetMemberAsync(username);
            return member;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername()); // Any updates to this user are going to be tracked by entity framework

            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDTO, user); // Updates all of the properties from MemberDTO to the user object - not persisted yet

            if (!await _userRepository.SaveAllAsync()) BadRequest("Failed to update user"); // If there as an error saving/persisting in database, let us know

            return NoContent(); // error 204 - everything ok, nothing to send back

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null)
            {
                return NotFound();
            }

            var result = await _photoService.AddPhotoAsync(file);
            
            // If we receive an error, return a bad request
            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true; // If this is the first photo they uploaded, make it their main photo

            user.Photos.Add(photo); // Add the photo to their profile regardless
            
            if (await _userRepository.SaveAllAsync()) return CreatedAtAction(nameof(GetUser), // The URL the new resource is at
                new { username = user.UserName}, // We tell them to get the new resource at GetUser (sends a 204)
                _mapper.Map<PhotoDto>(photo)) ; // We still send back the created resource
            // _mapper.Map<PhotoDto>(photo);

            return BadRequest("Problem adding photo");
            
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto (int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound(); // They submitted a photo Id that is not their ID

            if (photo.IsMain) return BadRequest("this is already your main photo"); // They are trying to make their main photo their main photo
            
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            
            if (currentMain != null) currentMain.IsMain = false; // Sets the previous main to false (if there is a previous main)
            photo.IsMain = true; // Sets the new main to true

            if (await _userRepository.SaveAllAsync()) return NoContent();// If there is no problem, return NoContent

            return BadRequest("Problem setting the main photo"); // If there is a problem saving, tell them
        }
        

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest ("You cannot delete your main photo");

            if (photo.PublicId != null) // If there is a publicId on the photo, delete it
            {
                var result = await _photoService.DeletePhotoAsnc(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message); // if Cloudinary service has a problem deleting a photo, return the error they return

            }

            user.Photos.Remove(photo); // Remove the photo from the user's photo list

            if (await _userRepository.SaveAllAsync()) return Ok(); // commit changes to database

            return BadRequest("Problem deleting photo"); // if couldn't save changes, problem deleting photo
        }
        
    }
}