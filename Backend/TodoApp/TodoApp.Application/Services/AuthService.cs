using TodoApp.Application.DTOs.Auth;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var exists = await _userRepository.ExistsAsync(dto.Email);
            if (exists)
                throw new Exception("User with this email already exists");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password)
            };

            await _userRepository.CreateAsync(user);

            return new AuthResponseDto
            {
                AccessToken = "", // в API
                RefreshToken = "",
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            return new AuthResponseDto
            {
                AccessToken = "", // в API
                RefreshToken = "",
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public Task LogoutAsync(string refreshToken)
        {
            return Task.CompletedTask;
        }
    }
}
