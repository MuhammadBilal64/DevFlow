using System;
using System.Collections.Generic;
using System.Text;
using BCrypt.Net;
using DevFlow.Application.Abstractions;

namespace DevFlow.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            return hash;
        }

        public bool verify(string pass, string hashedpassword)
        {
            bool matching= BCrypt.Net.BCrypt.Verify(pass, hashedpassword);
            return matching;

        }
    }
}
