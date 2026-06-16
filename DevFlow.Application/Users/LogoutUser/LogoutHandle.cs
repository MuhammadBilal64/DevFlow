using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;

namespace DevFlow.Application.Users.LogoutUser
{
    public class LogoutHandle
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public LogoutHandle(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task Handle(LogoutCommand command)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken);
            if (existingToken == null)
            {
                throw new UnAuthorizedException("Invalid refresh token");
            }
            if (existingToken.IsRevoked)
            {
                throw new UnAuthorizedException("Refresh token revoked");
            }
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
           await _refreshTokenRepository.UpdateAsync(existingToken);

        }
    }
}
