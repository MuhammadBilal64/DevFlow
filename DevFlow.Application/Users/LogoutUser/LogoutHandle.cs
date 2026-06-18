using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Users.LogoutUser
{
    public class LogoutHandler:IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public LogoutHandler(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async  Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (existingToken == null)
            {
                throw new UnauthorizedException("Invalid refresh token");
            }
            if (existingToken.IsRevoked)
            {
                throw new UnauthorizedException("Refresh token revoked");
            }
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
           await _refreshTokenRepository.UpdateAsync(existingToken);

        }

       
    }
}
