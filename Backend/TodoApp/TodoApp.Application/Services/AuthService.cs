using TodoApp.Application.DTOs.Auth;
using TodoApp.Application.Interfaces.Services;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Interfaces;
using TodoApp.Domain.Interfaces.Repositories;

namespace TodoApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
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

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
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

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _userRepository.AddRefreshTokenAsync(refreshToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            await _userRepository.RevokeRefreshTokenAsync(refreshToken);
        }
    }
}
