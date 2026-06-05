using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
