using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using MediatR;

namespace DevFlow.Application.Users.LogoutUser
{
    public class LogoutHandler:IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        public LogoutHandler(IUnitOfWork unitOfWork,ICurrentUserService currentUserService,IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }
        public async  Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (existingToken == null|| existingToken.IsRevoked|| existingToken.UserId != _currentUserService.UserId)
            {
                throw new UnauthorizedException("Invalid refresh token");
            }
            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
           await _refreshTokenRepository.UpdateAsync(existingToken);
            await _unitOfWork.SaveChangesAsync();
        }

       
    }
}
