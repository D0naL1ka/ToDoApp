using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsAsync(string email);
        Task AddRefreshTokenAsync(RefreshToken token);
        Task RevokeRefreshTokenAsync(string token);
    }
}
