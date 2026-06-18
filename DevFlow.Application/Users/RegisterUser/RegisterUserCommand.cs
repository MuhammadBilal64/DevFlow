using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace DevFlow.Application.Users.RegisterUser
{
    public class RegisterUserCommand:IRequest<RegisterUserResult>
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
