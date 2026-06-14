using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Users.LoginUser
{
    public class LoginUserResult
    {
       
        public string AccessToken { get; set; } = null!;
        public string RefreshToken {  get; set; } = null!;
      
    }
}
