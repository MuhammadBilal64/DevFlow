using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using DevFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevFlow.Infrastructure.Repositories
{
    public  class UserRepository:IUserRepository
    {
        private readonly DevFlowDbContext _context;

        public UserRepository(DevFlowDbContext context)
        {
            _context= context;
        }
        public async Task<bool> ExistByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);

        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

    }
}
