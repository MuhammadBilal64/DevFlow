using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Users.LoginUser
{
    public class LoginUserHandle
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public LoginUserHandle(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator,IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _refreshTokenRepository= refreshTokenRepository;

        }
        public async Task<LoginUserResult> Handle(LoginUserCommand command)
        {
            var user =  await _userRepository.GetByEmailAsync(command.Email);
            if (user == null)
            {
                throw new NotFoundException("User not Registered");
            }
            bool verify = _passwordHasher.verify(command.Password,user.PasswordHash );
            if (verify == false)
            {
                throw new UnauthorizedException("Invalid email or password");
            }
            var AccessToken_ = _jwtTokenGenerator.GenerateAccessToken(user);
            var RefreshToken_ = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshToken = new DevFlow.Domain.Entities.RefreshToken
            {
                Token = RefreshToken_,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(refreshToken);

            return new LoginUserResult
            {
                AccessToken= AccessToken_,
                RefreshToken= RefreshToken_,
            };

        }
    }
}
