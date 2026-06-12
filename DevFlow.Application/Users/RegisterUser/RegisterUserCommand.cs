using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Users.RegisterUser
{
    public class RegisterUserCommand
    {
       public string Name {  get; set; }
        public string Email { get; set; }
        public string Password {  get; set; }
    }
}
