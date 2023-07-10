using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
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
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            IEnumerable<MemberDto> users = await _userRepository.GetMembersAsync();

            return Ok(users);
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
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Optional null operator prevents exceptions from the value
            var user = await _userRepository.GetUserByUsernameAsync(username); // Any updates to this user are going to be tracked by entity framework

            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDTO, user); // Updates all of the properties from MemberDTO to the user object - not persisted yet

            if (!await _userRepository.SaveAllAsync()) BadRequest("Failed to update user"); // If there as an error saving/persisting in database, let us know

            return NoContent(); // error 204 - everything ok, nothing to send back

        }
    }
}