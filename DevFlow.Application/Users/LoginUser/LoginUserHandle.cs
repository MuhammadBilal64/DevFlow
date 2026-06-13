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
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public LoginUserHandle(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<LoginUserResult> Handle(LoginUserCommand command)
        {
            var user =  await _userRepository.GetByEmailAsync(command.Email);
            if (user == null)
            {
                throw new Exception("User not Registered");
            }
            bool verify = _passwordHasher.verify(command.Password,user.PasswordHash );
            if (verify == false)
            {
                throw new Exception("Password Not Matched");
            }
            var AccessToken_ = _jwtTokenGenerator.GenerateToken(user);

            return new LoginUserResult
            {
                AccessToken= AccessToken_
            };

        }
    }
}
