using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Users.LogoutUser
{
    public class LogoutCommand
    {
        public string RefreshToken { get; set; } = null!;

    }
}
