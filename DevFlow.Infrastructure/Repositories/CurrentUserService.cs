using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using DevFlow.Application.Common.Interfaces;
using DevFlow.Application.Exceptions;
using Microsoft.AspNetCore.Http;

namespace DevFlow.Infrastructure.Repositories
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor) {
            _contextAccessor = httpContextAccessor;
        }
        public int UserId
        {
            get
            {
                var user = _contextAccessor.HttpContext?.User;
                var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrWhiteSpace(userIdClaim) && int.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }
                throw new UnauthorizedException("User is not authenticated");


            }
        }
    }
}
