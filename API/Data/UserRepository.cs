using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.Execution;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            
        }

        public async Task<MemberDto> GetMemberAsync(string username) // Returns Member Dto
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) // Automapper projects user class to memberdto
                .SingleOrDefaultAsync(); 
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams) // Returns an ienumerable of memberdtos
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUsername); // excludes currently logged in user
            query = query.Where(u => u.Gender == userParams.Gender); // filter on the gender they are searching for

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1)); // lowest value year AKA how old they can be
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created), // If the userParams is "created" for ordering, do this
                _ => query.OrderByDescending(u => u.LastActive) // else do this (default)
            };
                
            return await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking()  // not necessary, just improves - entity framework doesn't track what we are returning, 
                    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider), // Project - doesn't need Include for eager loading, already loads
                userParams.PageNumber, 
                userParams.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
 
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Photos) // Eagerly load the photo URLs
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0; //  SaveChanges returns number of changes, returns whether it it is true or not
        }

        public async void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified; // Tells entity framework tracker that an entity has been updated
        }
    }
}