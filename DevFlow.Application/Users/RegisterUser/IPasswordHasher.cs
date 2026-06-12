using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Users.RegisterUser
{
    public interface IPasswordHasher
    {
        string  Hash(string password);
    }
}
