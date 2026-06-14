using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Users.RefreshToken
{
    public class RefreshTokenHandler
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        public RefreshTokenHandler(IRefreshTokenRepository refreshTokenRepository, IJwtTokenGenerator jwtTokenGenerator,IUserRepository userRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository= userRepository;
            
        }
        public async Task<RefreshTokenResult> Handle(RefreshTokenCommand command)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken);
            if (existingToken == null)
            {
                throw new Exception("Token Doesnot Exist");
            }
            if (existingToken.IsRevoked)
            {
                throw new Exception("Refresh Token Revoked");
            }
            if (existingToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Refresh Token Expired");
            }
            int  userId=existingToken.UserId;
            var user=await _userRepository.GetByUserIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not exist");
            }
            string newAccessToken=_jwtTokenGenerator.GenerateAccessToken(user);
            string newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.ReplacedByToken = newRefreshToken;

            await _refreshTokenRepository.UpdateAsync(existingToken);
            var refreshTokenEntity = new DevFlow.Domain.Entities.RefreshToken
            {
                Token = newRefreshToken,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,

            };
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            return new RefreshTokenResult
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

        }

    }
}
