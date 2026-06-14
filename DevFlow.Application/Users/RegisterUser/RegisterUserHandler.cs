using System;
using System.Collections.Generic;
using System.Text;
using DevFlow.Application.Abstractions;
using DevFlow.Domain.Entities;
using DevFlow.Domain.Enum;

namespace DevFlow.Application.Users.RegisterUser
{
    public class RegisterUserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        public RegisterUserHandler(IUserRepository userRepository,IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
      
        public async Task<RegisterUserResult>Handle(RegisterUserCommand command)
        {
            var exists = await _userRepository.ExistByEmailAsync(command.Email);
            if (exists)
            {
                throw new Exception("Email Already Exist");
               
            }
            var Password = _passwordHasher.Hash(command.Password);
            var user = new User
            {
                Name = command.Name,
                Email = command.Email,
                Role = UserRole.Member,
                CreatedAt = DateTime.UtcNow,
                PasswordHash=Password


            };
           await  _userRepository.AddAsync(user);
            return new RegisterUserResult
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,

            };

        }
    }
}
