using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Users.LoginUser
{
    public class LoginUserCommand:IRequest<LoginUserResult>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
