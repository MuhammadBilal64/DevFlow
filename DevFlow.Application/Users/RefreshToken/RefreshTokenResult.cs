using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Users.RefreshToken
{
    public class RefreshTokenResult
    {
        public string RefreshToken { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
    }
}
