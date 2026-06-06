using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;
using TodoApp.Infrastructure.Data;

namespace TodoApp.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);

        public async Task<bool> ExistsAsync(string email)
            => await _context.Users
                .AnyAsync(x => x.Email == email);

        public async Task AddRefreshTokenAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked);
            if (refreshToken == null) return;

            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }
}
