using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using DevFlow.Application.Common.Interfaces;
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
                if (userIdClaim != null)
                {
                    return int.Parse(userIdClaim);
                }
                else
                {
                    throw new UnauthorizedAccessException("User is not authenticated");
                }


            }
        }
    }
}
