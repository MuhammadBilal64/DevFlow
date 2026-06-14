using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Users.LoginUser
{
    public class LoginUserCommand
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
