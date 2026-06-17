using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using DevFlow.Domain.Entities;

namespace DevFlow.Application.Users.RefreshToken
{
    public class RefreshTokenHandle
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        public RefreshTokenHandle(IRefreshTokenRepository refreshTokenRepository, IJwtTokenGenerator jwtTokenGenerator,IUserRepository userRepository)
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
                throw new UnauthorizedException("Invalid refresh token");
            }
            if (existingToken.IsRevoked)
            {
                throw new UnauthorizedException("Refresh token revoked");
            }
            if (existingToken.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedException("Refresh Token Expired");
            }
            int  userId=existingToken.UserId;
            var user=await _userRepository.GetByUserIdAsync(userId);
            if (user == null)
            {
                throw new UnauthorizedException("User not exist");
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
