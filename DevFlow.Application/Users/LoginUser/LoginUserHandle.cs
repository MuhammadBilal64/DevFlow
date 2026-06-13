using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;

namespace DevFlow.Application.Users.LoginUser
{
    public class LoginUserHandle
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        public LoginUserHandle(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<LoginUserResult> Handle(LoginUserCommand command)
        {
            var user =  await _userRepository.GetByEmailAsync(command.Email);
            if (user == null)
            {
                throw new Exception("User not Registered");
            }
           var hashed = _passwordHasher.Hash(command.Password);
            bool verify = _passwordHasher.verify(user., hashed);
            if (verify == false)
            {
                throw new Exception("Password Not Matched");
            }
            return new LoginUserResult
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                Name = user.Name,
            };

        }
    }
}
