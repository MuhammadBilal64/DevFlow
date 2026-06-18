using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Users.LogoutUser
{
    public class LogoutCommand:IRequest
    {
        public string RefreshToken { get; set; } = null!;

    }
}
