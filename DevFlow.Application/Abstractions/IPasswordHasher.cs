using System;
using System.Collections.Generic;
using System.Text;

namespace DevFlow.Application.Abstractions
{
    public interface IPasswordHasher
    {
        string  Hash(string password);
        bool verify(string pass, string hashedpassword);
    }
}
