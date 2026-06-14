using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;

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
                throw new Exception("Token Doesnot Exist");
            }
            if (existingToken.IsRevoked)
            {
                throw new Exception("Token Revoked");
            }
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
           await _refreshTokenRepository.UpdateAsync(existingToken);

        }
    }
}
