using API.DTOs;
using API.Entities;
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

        public async Task<IEnumerable<MemberDto>> GetMembersAsync() // Returns an ienumerable of memberdtos
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider) // Project - doesn't need Include for eager loading, already loads
                .ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
 
        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Photos)
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