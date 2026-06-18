using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Users.RefreshToken
{
    public class RefreshTokenCommand:IRequest<RefreshTokenResult>
    {
        public string RefreshToken { get; set; } = null!;
    }
}
